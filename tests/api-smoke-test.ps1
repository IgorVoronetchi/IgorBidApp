<#
.SYNOPSIS
  Test de regresie automat pentru API-ul Bid-App-Igor.
  Loveste backend-ul direct (fara browser) si verifica fluxurile principale:
  auth, validare, CRUD item, bidding, wishlist, forum, recenzii, permisiuni.

.USAGE
  powershell -File tests/api-smoke-test.ps1
  powershell -File tests/api-smoke-test.ps1 -BaseUrl "https://your-backend.onrender.com"

.NOTE
  Presupune ca baza de date are datele demo originale din DbSeeder
  (radu@demo.ro / ana@demo.ro / parola123, igor@bidapp.ro / Igor123 admin).
  Creeaza propriile date de test (cu email unic per rulare) si nu le sterge
  la final (nu exista endpoint de delete) - ruleaza pe o baza demo, nu productie.
#>

param(
    [string]$BaseUrl = "http://localhost:5000"
)

$ErrorActionPreference = "Stop"
$api = "$BaseUrl/api"
$script:passCount = 0
$script:failCount = 0
$stamp = Get-Date -Format "yyyyMMddHHmmss"

function Test-Case {
    param([string]$Name, [scriptblock]$Body)
    try {
        & $Body
        Write-Host "  PASS  $Name" -ForegroundColor Green
        $script:passCount++
    } catch {
        Write-Host "  FAIL  $Name" -ForegroundColor Red
        Write-Host "        $($_.Exception.Message)" -ForegroundColor DarkYellow
        $script:failCount++
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Invoke-ExpectError {
    # Scris sa functioneze atat pe Windows PowerShell 5.1 (WebException) cat si pe
    # PowerShell 7+ (HttpResponseException) - ambele expun .Exception.Response.StatusCode.
    param([scriptblock]$Body, [int]$ExpectedStatus)
    try {
        & $Body
        throw "Expected HTTP $ExpectedStatus but the call succeeded."
    } catch {
        $resp = $_.Exception.Response
        if ($null -eq $resp) { throw }
        $actual = [int]$resp.StatusCode
        Assert-True ($actual -eq $ExpectedStatus) "Expected HTTP $ExpectedStatus, got $actual."
    }
}

Write-Host "`nBid-App-Igor API smoke test -> $BaseUrl`n" -ForegroundColor Cyan

# ---------- 1. Backend reachable ----------
Test-Case "Backend raspunde pe /api/item" {
    $r = Invoke-WebRequest -UseBasicParsing "$api/item" -TimeoutSec 10
    Assert-True ($r.StatusCode -eq 200) "Status code neasteptat: $($r.StatusCode)"
}

Test-Case "Categoriile sunt populate (seed data)" {
    $cats = Invoke-RestMethod "$api/category"
    Assert-True ($cats.Count -ge 5) "Ma asteptam la minim 5 categorii, am gasit $($cats.Count)."
}

# ---------- 2. Auth: register + login ----------
$testEmail = "smoketest_$stamp@test.ro"
$testUser = $null
$testToken = $null

Test-Case "Register cu date valide creeaza cont si intoarce JWT" {
    $script:testUser = Invoke-RestMethod -Method Post -Uri "$api/user/register" -ContentType "application/json" `
        -Body (@{ userName = "smoketest_$stamp"; name = "Smoke Test"; email = $testEmail; password = "SmokeTest123" } | ConvertTo-Json)
    Assert-True ($null -ne $script:testUser.token) "Lipseste tokenul din raspuns."
    $script:testToken = $script:testUser.token
}

Test-Case "Register cu parola prea scurta e respins (DataAnnotations)" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/user/register" -ContentType "application/json" `
            -Body (@{ userName = "shortpw_$stamp"; name = "X"; email = "shortpw_$stamp@test.ro"; password = "abc" } | ConvertTo-Json)
    }
}

Test-Case "Register cu email duplicat e respins" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/user/register" -ContentType "application/json" `
            -Body (@{ userName = "dup_$stamp"; name = "X"; email = $testEmail; password = "AnotherPass123" } | ConvertTo-Json)
    }
}

Test-Case "Login cu parola gresita e respins (401), fara sa scurga daca emailul exista" {
    Invoke-ExpectError -ExpectedStatus 401 {
        Invoke-RestMethod -Method Post -Uri "$api/user/login" -ContentType "application/json" `
            -Body (@{ email = $testEmail; password = "gresita" } | ConvertTo-Json)
    }
}

Test-Case "Login cu date corecte functioneaza" {
    $login = Invoke-RestMethod -Method Post -Uri "$api/user/login" -ContentType "application/json" `
        -Body (@{ email = $testEmail; password = "SmokeTest123" } | ConvertTo-Json)
    Assert-True ($login.user.email -eq $testEmail) "Emailul din raspuns nu corespunde."
}

Test-Case "GET /api/user/me necesita autentificare (401 fara token)" {
    Invoke-ExpectError -ExpectedStatus 401 {
        Invoke-RestMethod "$api/user/me"
    }
}

Test-Case "GET /api/user/me functioneaza cu tokenul valid" {
    $me = Invoke-RestMethod "$api/user/me" -Headers @{ Authorization = "Bearer $testToken" }
    Assert-True ($me.email -eq $testEmail) "Profilul intors nu corespunde userului logat."
}

# ---------- 3. Item lifecycle + admin approval ----------
$categories = Invoke-RestMethod "$api/category"
$catId = $categories[0].id
$newItemId = $null

Test-Case "Crearea unei listari fara autentificare e respinsa (401)" {
    Invoke-ExpectError -ExpectedStatus 401 {
        Invoke-RestMethod -Method Post -Uri "$api/item" -ContentType "application/json" `
            -Body (@{ name = "X"; startPrice = 10; categoryId = $catId; location = "X"; endDate = (Get-Date).AddDays(1).ToString("o") } | ConvertTo-Json)
    }
}

Test-Case "Crearea unei listari valide intra in coada (status Added)" {
    $item = Invoke-RestMethod -Method Post -Uri "$api/item" -ContentType "application/json" `
        -Headers @{ Authorization = "Bearer $testToken" } `
        -Body (@{ name = "Smoke Test Item $stamp"; startPrice = 42; categoryId = $catId; location = "Test City"; endDate = (Get-Date).AddDays(2).ToString("o") } | ConvertTo-Json)
    Assert-True ($item.status -eq "Added") "Status asteptat 'Added', primit '$($item.status)'."
    $script:newItemId = $item.id
}

Test-Case "Un user non-admin nu poate aproba listari (403)" {
    Invoke-ExpectError -ExpectedStatus 403 {
        Invoke-RestMethod -Method Post -Uri "$api/item/$newItemId/approve" -Headers @{ Authorization = "Bearer $testToken" }
    }
}

$adminLogin = Invoke-RestMethod -Method Post -Uri "$api/user/login" -ContentType "application/json" `
    -Body (@{ email = "igor@bidapp.ro"; password = "Igor123" } | ConvertTo-Json)
$adminToken = $adminLogin.token

Test-Case "Adminul vede noua listare in coada de validare" {
    $pending = Invoke-RestMethod "$api/item/pending" -Headers @{ Authorization = "Bearer $adminToken" }
    # @() forteaza rezultatul ca array, altfel un singur match nu are .Count in Windows PowerShell 5.1
    $match = @($pending | Where-Object { $_.id -eq $newItemId })
    Assert-True ($match.Count -eq 1) "Listarea noua nu apare in coada."
}

Test-Case "Adminul aproba listarea si devine vizibila public" {
    $result = Invoke-RestMethod -Method Post -Uri "$api/item/$newItemId/approve" -Headers @{ Authorization = "Bearer $adminToken" }
    Assert-True ($result.status -in @("ActiveBid", "Validated")) "Status neasteptat dupa aprobare: $($result.status)."
    $publicItem = Invoke-RestMethod "$api/item/$newItemId"
    Assert-True ($publicItem.name -like "Smoke Test Item*") "Itemul aprobat nu se regaseste public."
}

# ---------- 4. Bidding ----------
$raduLogin = Invoke-RestMethod -Method Post -Uri "$api/user/login" -ContentType "application/json" `
    -Body (@{ email = "radu@demo.ro"; password = "parola123" } | ConvertTo-Json)
$raduToken = $raduLogin.token

Test-Case "Nu poti licita pe propriul obiect" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/bid" -ContentType "application/json" `
            -Headers @{ Authorization = "Bearer $testToken" } -Body (@{ itemId = $newItemId; amount = 999 } | ConvertTo-Json)
    }
}

Test-Case "O oferta sub pretul curent e respinsa" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/bid" -ContentType "application/json" `
            -Headers @{ Authorization = "Bearer $raduToken" } -Body (@{ itemId = $newItemId; amount = 1 } | ConvertTo-Json)
    }
}

Test-Case "O oferta valida e acceptata si actualizeaza pretul curent" {
    $bidResult = Invoke-RestMethod -Method Post -Uri "$api/bid" -ContentType "application/json" `
        -Headers @{ Authorization = "Bearer $raduToken" } -Body (@{ itemId = $newItemId; amount = 100 } | ConvertTo-Json)
    Assert-True ($bidResult.currentPrice -eq 100) "Pretul curent nu s-a actualizat la 100."
}

# ---------- 5. Wishlist ----------
Test-Case "Toggle wishlist adauga si scoate corect itemul" {
    $add = Invoke-RestMethod -Method Post -Uri "$api/wishlist/$newItemId" -Headers @{ Authorization = "Bearer $raduToken" }
    Assert-True ($add.inWishlist -eq $true) "Itemul nu a fost adaugat in wishlist."
    $remove = Invoke-RestMethod -Method Post -Uri "$api/wishlist/$newItemId" -Headers @{ Authorization = "Bearer $raduToken" }
    Assert-True ($remove.inWishlist -eq $false) "Itemul nu a fost scos din wishlist."
}

# ---------- 6. Forum ----------
$postId = $null
Test-Case "Postarea pe forum fara autentificare e respinsa (401)" {
    Invoke-ExpectError -ExpectedStatus 401 {
        Invoke-RestMethod -Method Post -Uri "$api/forum" -ContentType "application/json" `
            -Body (@{ title = "X"; description = "Y" } | ConvertTo-Json)
    }
}

Test-Case "Postarea pe forum autentificat functioneaza" {
    $post = Invoke-RestMethod -Method Post -Uri "$api/forum" -ContentType "application/json" `
        -Headers @{ Authorization = "Bearer $testToken" } -Body (@{ title = "Smoke test post $stamp"; description = "Automated check." } | ConvertTo-Json)
    $script:postId = $post.id
    Assert-True ($null -ne $postId) "Nu s-a primit id de postare."
}

Test-Case "Comentariul pe postare functioneaza" {
    $comment = Invoke-RestMethod -Method Post -Uri "$api/forum/$postId/comments" -ContentType "application/json" `
        -Headers @{ Authorization = "Bearer $raduToken" } -Body (@{ text = "Automated comment." } | ConvertTo-Json)
    Assert-True ($null -ne $comment.id) "Nu s-a primit id de comentariu."
}

# ---------- 7. Reviews ----------
Test-Case "Nu poti sa iti lasi singur recenzie" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/review" -ContentType "application/json" `
            -Headers @{ Authorization = "Bearer $testToken" } -Body (@{ reviewedUserId = $testUser.user.id; rating = 5; comment = "self" } | ConvertTo-Json)
    }
}

Test-Case "O recenzie valida pentru alt user functioneaza" {
    $review = Invoke-RestMethod -Method Post -Uri "$api/review" -ContentType "application/json" `
        -Headers @{ Authorization = "Bearer $raduToken" } -Body (@{ reviewedUserId = $testUser.user.id; rating = 5; comment = "Smoke test review." } | ConvertTo-Json)
    Assert-True ($null -ne $review.id) "Nu s-a primit id de recenzie."
}

Test-Case "Rating-ul mediu global se poate citi" {
    $stats = Invoke-RestMethod "$api/review/stats"
    Assert-True ($stats.count -gt 0) "Contorul de recenzii ar trebui sa fie > 0."
}

# ---------- 8. Image upload restriction ----------
Test-Case "Upload de imagine non-JPEG (ex. SVG) e respins" {
    Invoke-ExpectError -ExpectedStatus 400 {
        Invoke-RestMethod -Method Post -Uri "$api/item" -ContentType "application/json" `
            -Headers @{ Authorization = "Bearer $testToken" } `
            -Body (@{ name = "Bad image $stamp"; startPrice = 10; categoryId = $catId; location = "X"; endDate = (Get-Date).AddDays(1).ToString("o"); imageDataUrl = "data:image/svg+xml;base64,PHN2Zz48L3N2Zz4=" } | ConvertTo-Json)
    }
}

# ---------- Summary ----------
Write-Host "`n----------------------------------------"
Write-Host "PASS: $script:passCount   FAIL: $script:failCount" -ForegroundColor $(if ($script:failCount -eq 0) { "Green" } else { "Red" })
Write-Host "----------------------------------------`n"

if ($script:failCount -gt 0) { exit 1 } else { exit 0 }
