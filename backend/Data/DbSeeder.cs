using Backend.Models;
using Backend.Services;

namespace Backend.Data
{
    public static class DbSeeder
    {
        /// <summary>
        /// Poza demo de pe Unsplash (licenta libera, hotlink permis pe CDN-ul lor).
        /// Stocam doar URL-ul in baza de date; octetii imaginii raman pe CDN.
        /// Parametrii cer o varianta redimensionata la 800px, comprimata - suficient pentru carduri.
        /// </summary>
        private static string Unsplash(string photoId) =>
            $"https://images.unsplash.com/{photoId}?w=800&q=80&auto=format&fit=crop";

        public static void Seed(ApplicationDbContext db)
        {
            if (db.Users.Any()) return; // deja populat

            var now = DateTime.UtcNow;
            string demoPass = PasswordHasher.HashPassword("parola123");

            var admin = new User { UserName = "igor", Name = "Igor", Email = "igor@bidapp.ro", Password = PasswordHasher.HashPassword("Igor123"), Role = RoleEnum.Admin, MemberSince = now.AddMonths(-8) };
            var vlad = new User { UserName = "foto_vlad", Name = "Vlad Popescu", Email = "vlad@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-5) };
            var galeria = new User { UserName = "galeria_9", Name = "Galeria Noua", Email = "galeria@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-4) };
            var clasice = new User { UserName = "clasice_ro", Name = "Clasice RO", Email = "clasice@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-4) };
            var tech = new User { UserName = "tech_resale", Name = "Tech Resale", Email = "tech@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-3) };
            var ana = new User { UserName = "ana.k", Name = "Ana Kovacs", Email = "ana@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-3) };
            var radu = new User { UserName = "radu_m", Name = "Radu Marin", Email = "radu@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-2) };
            var ioana = new User { UserName = "ioana.d", Name = "Ioana Dinu", Email = "ioana@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-2) };
            var bikeLife = new User { UserName = "bike_life", Name = "Stefan Bicicleta", Email = "bike@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-1) };
            var watchVault = new User { UserName = "watch_vault", Name = "Watch Vault", Email = "watch@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-5) };
            var vinylBar = new User { UserName = "vinyl_bar", Name = "Vinyl Bar", Email = "vinyl@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-2) };
            var mihai = new User { UserName = "mihai_c", Name = "Mihai Constantin", Email = "mihai@demo.ro", Password = demoPass, Role = RoleEnum.User, MemberSince = now.AddMonths(-1) };

            db.Users.AddRange(admin, vlad, galeria, clasice, tech, ana, radu, ioana, bikeLife, watchVault, vinylBar, mihai);

            var electronics = new CategoryItem { Name = "Electronics" };
            var art = new CategoryItem { Name = "Art" };
            var auto = new CategoryItem { Name = "Auto" };
            var collectibles = new CategoryItem { Name = "Collectibles" };
            var fashion = new CategoryItem { Name = "Fashion" };
            db.Categories.AddRange(electronics, art, auto, collectibles, fashion);
            db.SaveChanges();

            var camera = new AuctionItem
            {
                Name = "Vintage Zenit-E Film Camera", Category = electronics, StartPrice = 150, CurrentPrice = 320,
                StartDate = now.AddDays(-3), EndDate = now.AddHours(2).AddMinutes(14), Location = "Cluj-Napoca",
                Description = "Fully working Soviet-era 35mm SLR with Helios 44-2 lens. Light meter functional, minor cosmetic wear on the top plate. Comes with original leather case.",
                Owner = vlad, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1495707902641-75cac588d2e9")
            };
            var poster = new AuctionItem
            {
                Name = "Signed Exhibition Poster, 2019", Category = art, StartPrice = 800, CurrentPrice = 1450,
                StartDate = now.AddDays(-5), EndDate = now.AddMinutes(45), Location = "Bucuresti",
                Description = "Limited exhibition poster, hand-signed by the artist. 70x100 cm, kept flat in acid-free sleeve. Certificate of authenticity included.",
                Owner = galeria, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1561214115-f2f134cc4912")
            };
            var dacia = new AuctionItem
            {
                Name = "1987 Dacia 1300, restored", Category = auto, StartPrice = 12000, CurrentPrice = 18500,
                StartDate = now.AddDays(-7), EndDate = now.AddDays(3).AddHours(4), Location = "Pitesti",
                Description = "Full nut-and-bolt restoration finished in 2025. Original engine rebuilt, new interior in period-correct fabric, ITP valid until 2027. A genuine head-turner.",
                Owner = clasice, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1489824904134-891ab64532f1")
            };
            var iphone = new AuctionItem
            {
                Name = "iPhone 13 Pro 256GB, Sierra Blue", Category = electronics, StartPrice = 1800, CurrentPrice = 2350,
                StartDate = now.AddDays(-2), EndDate = now.AddHours(6).AddMinutes(30), Location = "Timisoara",
                Description = "Battery health 89%, always in case with screen protector. Box, cable and invoice included. No scratches, no repairs.",
                Owner = tech, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1592750475338-74b7b21085ab")
            };
            var rug = new AuctionItem
            {
                Name = "Handwoven Maramures Wool Rug", Category = art, StartPrice = 400, CurrentPrice = 640,
                StartDate = now.AddDays(-4), EndDate = now.AddDays(1).AddHours(2), Location = "Baia Mare",
                Description = "Traditional 160x230 cm rug, hand-woven with natural dyes by a local artisan. Never used, from a smoke-free home.",
                Owner = ioana, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1600166898405-da9535204843")
            };
            var omega = new AuctionItem
            {
                Name = "Omega Seamaster, 1970s", Category = collectibles, StartPrice = 5000, CurrentPrice = 7200,
                StartDate = now.AddDays(-6), EndDate = now.AddHours(5).AddMinutes(12), Location = "Bucuresti",
                Description = "Automatic cal. 1012, recently serviced with papers. Original dial with beautiful patina, 36mm steel case. Runs +4s/day.",
                Owner = watchVault, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1547996160-81dfa63595aa")
            };
            var pegas = new AuctionItem
            {
                Name = "Retro Pegas Bicycle, restored", Category = collectibles, StartPrice = 700, CurrentPrice = 980,
                StartDate = now.AddDays(-3), EndDate = now.AddHours(12).AddMinutes(45), Location = "Iasi",
                Description = "Classic Pegas frame repainted in original teal, new tyres, chain and Brooks-style saddle. Rides like new, looks like 1985.",
                Owner = bikeLife, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1485965120184-e220f721d03e")
            };
            var bag = new AuctionItem
            {
                Name = "Handmade Leather Messenger Bag", Category = fashion, StartPrice = 250, CurrentPrice = 310,
                StartDate = now.AddDays(-1), EndDate = now.AddHours(3).AddMinutes(31), Location = "Brasov",
                Description = "Full-grain vegetable-tanned leather, hand-stitched. Fits a 14\" laptop. Brass hardware, ages beautifully.",
                Owner = galeria, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1547949003-9792a18a2601")
            };
            var vespa = new AuctionItem
            {
                Name = "Vespa PX 150, 1998", Category = auto, StartPrice = 2200, CurrentPrice = 2850,
                StartDate = now.AddDays(-2), EndDate = now.AddHours(8).AddMinutes(20), Location = "Constanta",
                Description = "Classic two-stroke Vespa, recently serviced, new battery and tyres. Romanian title, ready to ride this summer.",
                Owner = clasice, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1558981403-c5f9899a28bc")
            };
            var switchConsole = new AuctionItem
            {
                Name = "Nintendo Switch OLED + 4 games", Category = electronics, StartPrice = 900, CurrentPrice = 1150,
                StartDate = now.AddDays(-1), EndDate = now.AddHours(4).AddMinutes(50), Location = "Oradea",
                Description = "White OLED model, barely used, includes Zelda TOTK, Mario Kart 8, Animal Crossing and a third-party dock.",
                Owner = mihai, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1578303512597-81e6cc155b3e")
            };
            var vinylCollection = new AuctionItem
            {
                Name = "Vinyl Record Collection (45 LPs)", Category = collectibles, StartPrice = 600, CurrentPrice = 870,
                StartDate = now.AddDays(-3), EndDate = now.AddDays(2).AddHours(6), Location = "Bucuresti",
                Description = "70s-80s rock and jazz, mostly VG+/NM condition, stored upright in a smoke-free room. Full list on request.",
                Owner = vinylBar, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1603048588665-791ca8aea617")
            };
            var coat = new AuctionItem
            {
                Name = "Designer Wool Coat, size M", Category = fashion, StartPrice = 350, CurrentPrice = 490,
                StartDate = now.AddDays(-2), EndDate = now.AddDays(1).AddHours(9), Location = "Bucuresti",
                Description = "Tailored wool-blend coat, worn twice, dry-cleaned. Original garment bag and spare buttons included.",
                Owner = ioana, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1539533018447-63fcce2678e3")
            };
            var painting = new AuctionItem
            {
                Name = "Abstract Oil Painting, 100x80cm", Category = art, StartPrice = 1200, CurrentPrice = 1650,
                StartDate = now.AddDays(-4), EndDate = now.AddDays(2).AddHours(14), Location = "Cluj-Napoca",
                Description = "Original oil on canvas, contemporary Romanian artist, gallery-framed and ready to hang. Certificate included.",
                Owner = galeria, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1541961017774-22349e4a1262")
            };
            var keyboard = new AuctionItem
            {
                Name = "Hand-built Mechanical Keyboard", Category = electronics, StartPrice = 500, CurrentPrice = 640,
                StartDate = now.AddDays(-1), EndDate = now.AddDays(3).AddHours(1), Location = "Timisoara",
                Description = "Custom 65% aluminium case, lubed switches, PBT keycaps. Hand-assembled, sounds and feels premium.",
                Owner = tech, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1595225476474-87563907a212")
            };
            var jeans = new AuctionItem
            {
                Name = "Vintage Levi's 501, rare wash", Category = fashion, StartPrice = 180, CurrentPrice = 245,
                StartDate = now.AddDays(-1), EndDate = now.AddDays(4).AddHours(5), Location = "Brasov",
                Description = "Made in USA, 90s deadstock, W32/L34. Rare mid-blue wash, never hemmed, original red tab.",
                Owner = mihai, Status = StatusEnum.ActiveBid, ImageDataUrl = Unsplash("photo-1542272604-787c3835535d")
            };
            // Licitatii deja incheiate, cu castigator — pentru profil/istoric
            var lego = new AuctionItem
            {
                Name = "LEGO Technic 42083 sealed", Category = collectibles, StartPrice = 1200, CurrentPrice = 1520,
                StartDate = now.AddDays(-10), EndDate = now.AddDays(-2), Location = "Cluj-Napoca",
                Description = "Bugatti Chiron, sealed box, shelf-kept. Retired set.",
                Owner = tech, Status = StatusEnum.Sold, Winner = radu, ImageDataUrl = Unsplash("photo-1585366119957-e9730b6d0f60")
            };
            var camera2 = new AuctionItem
            {
                Name = "Canon EOS 90D body only", Category = electronics, StartPrice = 2200, CurrentPrice = 2680,
                StartDate = now.AddDays(-9), EndDate = now.AddDays(-1), Location = "Bucuresti",
                Description = "Low shutter count, always used with a cage and ND filters. Boxed with two batteries.",
                Owner = tech, Status = StatusEnum.Sold, Winner = ana, ImageDataUrl = Unsplash("photo-1519638831568-d9897f54ed69")
            };
            // In asteptarea validarii adminului
            var ps5 = new AuctionItem
            {
                Name = "PS5 Console + 2 DualSense controllers", Category = electronics, StartPrice = 1500, CurrentPrice = 1500,
                StartDate = now, EndDate = now.AddDays(4), Location = "Bucuresti",
                Description = "Disc edition, lightly used, both controllers included with charging dock.",
                Owner = radu, Status = StatusEnum.Added, ImageDataUrl = Unsplash("photo-1606813907291-d86efa9b94db")
            };
            var spoons = new AuctionItem
            {
                Name = "Antique Silver Spoon Set (12 pcs)", Category = collectibles, StartPrice = 900, CurrentPrice = 900,
                StartDate = now, EndDate = now.AddDays(5), Location = "Sibiu",
                Description = "Hallmarked 800 silver, early 20th century, original wooden case.",
                Owner = ioana, Status = StatusEnum.Added, ImageDataUrl = Unsplash("photo-1608039829572-78524f79c4c7")
            };
            var ski = new AuctionItem
            {
                Name = "Ski Set Atomic 170cm + boots 43", Category = fashion, StartPrice = 600, CurrentPrice = 600,
                StartDate = now, EndDate = now.AddDays(6), Location = "Predeal",
                Description = "One season used, freshly waxed, bindings serviced.",
                Owner = bikeLife, Status = StatusEnum.Added, ImageDataUrl = Unsplash("photo-1551698618-1dfe5d97d256")
            };
            var sculpture = new AuctionItem
            {
                Name = "Small Bronze Sculpture, signed", Category = art, StartPrice = 700, CurrentPrice = 700,
                StartDate = now, EndDate = now.AddDays(7), Location = "Bucuresti",
                Description = "Limited edition cast, numbered 4/25, artist signature on the base.",
                Owner = galeria, Status = StatusEnum.Added, ImageDataUrl = Unsplash("photo-1544413660-299165566b1d")
            };

            db.AuctionItems.AddRange(
                camera, poster, dacia, iphone, rug, omega, pegas, bag, vespa, switchConsole,
                vinylCollection, coat, painting, keyboard, jeans, lego, camera2, ps5, spoons, ski, sculpture
            );
            db.SaveChanges();

            db.Bids.AddRange(
                new Bid { Item = camera, Bidder = ana, Amount = 200, PlacedAt = now.AddHours(-8) },
                new Bid { Item = camera, Bidder = radu, Amount = 280, PlacedAt = now.AddHours(-3) },
                new Bid { Item = camera, Bidder = ana, Amount = 320, PlacedAt = now.AddMinutes(-22) },

                new Bid { Item = poster, Bidder = ioana, Amount = 900, PlacedAt = now.AddHours(-5) },
                new Bid { Item = poster, Bidder = radu, Amount = 1100, PlacedAt = now.AddHours(-2) },
                new Bid { Item = poster, Bidder = ana, Amount = 1300, PlacedAt = now.AddMinutes(-40) },
                new Bid { Item = poster, Bidder = ioana, Amount = 1450, PlacedAt = now.AddMinutes(-9) },

                new Bid { Item = dacia, Bidder = radu, Amount = 13500, PlacedAt = now.AddDays(-2) },
                new Bid { Item = dacia, Bidder = ana, Amount = 16000, PlacedAt = now.AddDays(-1) },
                new Bid { Item = dacia, Bidder = radu, Amount = 18500, PlacedAt = now.AddHours(-6) },

                new Bid { Item = iphone, Bidder = ioana, Amount = 1850, PlacedAt = now.AddHours(-10) },
                new Bid { Item = iphone, Bidder = radu, Amount = 2000, PlacedAt = now.AddHours(-7) },
                new Bid { Item = iphone, Bidder = ana, Amount = 2200, PlacedAt = now.AddHours(-4) },
                new Bid { Item = iphone, Bidder = radu, Amount = 2350, PlacedAt = now.AddHours(-1) },

                new Bid { Item = rug, Bidder = ana, Amount = 520, PlacedAt = now.AddHours(-14) },
                new Bid { Item = rug, Bidder = radu, Amount = 640, PlacedAt = now.AddHours(-5) },

                new Bid { Item = omega, Bidder = radu, Amount = 5200, PlacedAt = now.AddHours(-12) },
                new Bid { Item = omega, Bidder = ana, Amount = 6000, PlacedAt = now.AddHours(-6) },
                new Bid { Item = omega, Bidder = ioana, Amount = 6800, PlacedAt = now.AddHours(-2) },
                new Bid { Item = omega, Bidder = ana, Amount = 7200, PlacedAt = now.AddMinutes(-30) },

                new Bid { Item = pegas, Bidder = ana, Amount = 850, PlacedAt = now.AddHours(-9) },
                new Bid { Item = pegas, Bidder = ioana, Amount = 980, PlacedAt = now.AddHours(-2) },

                new Bid { Item = bag, Bidder = radu, Amount = 270, PlacedAt = now.AddHours(-4) },
                new Bid { Item = bag, Bidder = ana, Amount = 310, PlacedAt = now.AddHours(-1) },

                new Bid { Item = vespa, Bidder = mihai, Amount = 2500, PlacedAt = now.AddHours(-20) },
                new Bid { Item = vespa, Bidder = radu, Amount = 2850, PlacedAt = now.AddHours(-3) },

                new Bid { Item = switchConsole, Bidder = ana, Amount = 950, PlacedAt = now.AddHours(-15) },
                new Bid { Item = switchConsole, Bidder = ioana, Amount = 1050, PlacedAt = now.AddHours(-6) },
                new Bid { Item = switchConsole, Bidder = mihai, Amount = 1150, PlacedAt = now.AddHours(-1) },

                new Bid { Item = vinylCollection, Bidder = radu, Amount = 700, PlacedAt = now.AddDays(-2) },
                new Bid { Item = vinylCollection, Bidder = ana, Amount = 870, PlacedAt = now.AddHours(-10) },

                new Bid { Item = coat, Bidder = ana, Amount = 400, PlacedAt = now.AddDays(-1) },
                new Bid { Item = coat, Bidder = ioana, Amount = 490, PlacedAt = now.AddHours(-5) },

                new Bid { Item = painting, Bidder = radu, Amount = 1400, PlacedAt = now.AddDays(-3) },
                new Bid { Item = painting, Bidder = mihai, Amount = 1650, PlacedAt = now.AddHours(-8) },

                new Bid { Item = keyboard, Bidder = ana, Amount = 560, PlacedAt = now.AddHours(-18) },
                new Bid { Item = keyboard, Bidder = radu, Amount = 640, PlacedAt = now.AddHours(-2) },

                new Bid { Item = jeans, Bidder = ioana, Amount = 210, PlacedAt = now.AddHours(-14) },
                new Bid { Item = jeans, Bidder = mihai, Amount = 245, PlacedAt = now.AddHours(-4) },

                new Bid { Item = lego, Bidder = radu, Amount = 1520, PlacedAt = now.AddDays(-3) },
                new Bid { Item = lego, Bidder = ana, Amount = 1350, PlacedAt = now.AddDays(-4) },

                new Bid { Item = camera2, Bidder = ana, Amount = 2450, PlacedAt = now.AddDays(-2) },
                new Bid { Item = camera2, Bidder = mihai, Amount = 2680, PlacedAt = now.AddDays(-1).AddHours(-3) }
            );

            db.Reviews.AddRange(
                new Review { Reviewer = galeria, ReviewedUser = radu, Rating = 5, Comment = "Paid instantly, great communication. Would gladly have them bid on my auctions again.", CreatedAt = now.AddDays(-14) },
                new Review { Reviewer = tech, ReviewedUser = radu, Rating = 5, Comment = "Smooth transaction from start to finish. Recommended buyer.", CreatedAt = now.AddDays(-32) },
                new Review { Reviewer = clasice, ReviewedUser = radu, Rating = 4, Comment = "Good buyer, slightly slow to confirm pickup but everything worked out.", CreatedAt = now.AddDays(-47) },
                new Review { Reviewer = ana, ReviewedUser = vlad, Rating = 5, Comment = "Camera exactly as described, well packed. Trustworthy seller!", CreatedAt = now.AddDays(-20) },
                new Review { Reviewer = radu, ReviewedUser = watchVault, Rating = 5, Comment = "The watch arrived with all papers, serviced as promised.", CreatedAt = now.AddDays(-10) },
                new Review { Reviewer = ioana, ReviewedUser = tech, Rating = 4, Comment = "Phone battery slightly below stated but fair price overall.", CreatedAt = now.AddDays(-5) },
                new Review { Reviewer = mihai, ReviewedUser = clasice, Rating = 5, Comment = "Vespa ran perfectly on the ride home, exactly as described.", CreatedAt = now.AddDays(-6) },
                new Review { Reviewer = ana, ReviewedUser = tech, Rating = 5, Comment = "Canon body arrived double-boxed, mint condition. Great seller.", CreatedAt = now.AddDays(-3) }
            );

            var post1 = new ForumPost
            {
                Title = "How do you spot a fair reserve price?",
                Description = "I keep seeing items start way below market value. Is that a strategy to attract bids, or am I missing something about how sellers set StartPrice here?",
                Author = radu, CreatedAt = now.AddDays(-2)
            };
            var post2 = new ForumPost
            {
                Title = "Won my first auction — the Pegas bike!",
                Description = "Sniped it in the last 40 seconds. Seller shipped in 2 days, exactly as described. This community is great.",
                Author = bikeLife, CreatedAt = now.AddDays(-4)
            };
            var post3 = new ForumPost
            {
                Title = "Feature request: photo uploads for listings",
                Description = "Placeholders are fine for now but real photos would help a lot when judging condition. Anyone else want this prioritized?",
                Author = ioana, CreatedAt = now.AddDays(-7)
            };
            db.ForumPosts.AddRange(post1, post2, post3);

            db.ForumComments.AddRange(
                new ForumComment { Post = post1, Author = watchVault, Text = "Low start = more early bidders = momentum. The market corrects the price by the end, almost always.", CreatedAt = now.AddDays(-2).AddHours(2) },
                new ForumComment { Post = post1, Author = ana, Text = "Check the seller rating first. Good sellers with low starts are the best deals on the platform.", CreatedAt = now.AddDays(-1) },
                new ForumComment { Post = post2, Author = bikeLife, Text = "Pleasure doing business! Enjoy the ride.", CreatedAt = now.AddDays(-3) },
                new ForumComment { Post = post3, Author = mihai, Text = "Just tried it on a new listing — huge improvement, items feel so much more trustworthy with a real photo.", CreatedAt = now.AddDays(-1) }
            );

            db.WishlistEntries.AddRange(
                new WishlistEntry { User = radu, Item = camera },
                new WishlistEntry { User = radu, Item = rug },
                new WishlistEntry { User = mihai, Item = painting },
                new WishlistEntry { User = ana, Item = vespa }
            );

            db.SaveChanges();
        }
    }
}
