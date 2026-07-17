import { Injectable, computed, signal } from '@angular/core';

const DICTS = {
  en: {
    navHome: 'Home', navAuctions: 'Auctions', navForum: 'Forum', navAdmin: 'Admin',
    logout: 'Log out', signIn: 'Sign in', joinFree: 'Join free',
    liveNow: 'live auctions right now',
    heroA: 'Bid fresh.', heroB: 'Win', heroC: 'big',
    heroSub: 'The community marketplace where every item finds its highest bidder. List it, bid on it, win it.',
    browseAuctions: 'Browse auctions', sellItem: 'Sell an item',
    statBidsLabel: 'bids placed today', statRatingLabel: 'avg. seller rating', statTradedLabel: 'traded across live listings',
    endingSoon: 'Ending soon', seeAll: 'See all auctions →', photo: 'photo',
    liveAuctionsTitle: 'Live auctions', searchPh: 'Search items…',
    sortEnding: 'Ending soonest', sortAsc: 'Price: low → high', sortDesc: 'Price: high → low', sortNew: 'Newest',
    itemsWord: 'items', bidsWord: 'bids',
    back: 'Back to auctions', description: 'Description', startedAt: 'Started at',
    completedSales: 'completed sales', viewProfile: 'View profile',
    currentBid: 'Current bid', startedAtLower: 'started at', timeLeftLabel: 'Time left', bidsSoFar: 'bids so far',
    endedSoldTo: 'Auction ended — sold to', endedNoWinner: 'Auction ended with no bids.',
    placeBid: 'Place bid', bidHistory: 'Bid history',
    noBids: 'No bids yet — be the first!', highest: 'HIGHEST',
    username: 'Username', password: 'Password', fullName: 'Full name',
    memberSince: 'Member since', reviewsWord: 'reviews',
    forumTitle: 'Community forum', forumSub: 'Tips, questions and stories from fellow bidders.',
    postTitlePh: 'Post title…', postBodyPh: "What's on your mind?", publish: 'Publish',
    replyPh: 'Write a reply…', replyBtn: 'Reply',
    adminTitle: 'Item validation', pendingWord: 'pending',
    adminSub: 'New listings wait here until an admin approves them. Rejected items never go live.',
    queueEmpty: 'Queue is empty — all caught up ✓', listedBy: 'listed by', startsAt: 'starts at',
    approve: 'Approve', reject: 'Reject',
    footerNote: 'Demo project — Angular + .NET',
    ended: 'Ended', minAgo: 'min ago', hAgo: 'h ago', dAgo: 'd ago',
    authTitleReg: 'Create your account', authTitleLogin: 'Welcome back',
    authSubReg: 'Join the community — list, bid, win.', authSubLogin: 'Sign in to bid and manage your listings.',
    authCtaReg: 'Create account', authCtaLogin: 'Sign in',
    authSwapTextReg: 'Already have an account?', authSwapTextLogin: 'New to Bid-App-Igor?',
    authSwapLinkReg: 'Sign in', authSwapLinkLogin: 'Create one',
    fillAll: 'Please fill in all fields.',
    signInToBid: 'Sign in to place a bid.', signInToPost: 'Sign in to post on the forum.',
    enterBid: 'Enter a bid amount.',
    youHighest: '✓ You are the highest bidder at {p}!',
    inWishlist: '💚 In wishlist', addWishlist: '🤍 Add to wishlist',
    tabMyItems: 'My items', tabWon: 'Won', tabWishlist: 'Wishlist', tabReviews: 'Reviews',
    badgeLive: 'LIVE', badgeWon: 'WON 🏆', badgePending: 'PENDING', badgeSold: 'SOLD',
    badgeRejected: 'REJECTED', badgeNoWinner: 'NO WINNER', badgeScheduled: 'SCHEDULED',
    emptyItems: 'You have no active listings.', emptyWon: 'No wins yet — keep bidding!',
    emptyWish: 'Your wishlist is empty. Tap the heart on any auction.', emptyReviews: 'No reviews yet.',
    hideReplies: 'Hide replies', replies: 'replies', reply: 'Reply',
    approvedMsg: '✓ "{n}" approved — it is now live in Auctions.',
    rejectedMsg: '✕ "{n}" rejected and removed from the queue.',
    sellTitle: 'Sell an item', sellSub: 'Your listing will go live after an admin validates it.',
    itemName: 'Item name', category: 'Category', location: 'Location',
    startPrice: 'Start price (lei)', duration: 'Duration', days: 'days',
    submitListing: 'Submit for validation',
    sellOk: '✓ Listing submitted! An admin will review it shortly.',
    yourReview: 'Leave the seller a review', rating: 'Rating', commentPh: 'How was the transaction?',
    sendReview: 'Send review', reviewOk: '✓ Review sent. Thank you!',
    winner: 'Winner', catAll: 'All',
    photoLabel: 'Photo (optional)', addPhoto: 'Add a photo', changePhoto: 'Change photo', removePhoto: 'Remove',
    imageTypeError: 'Please choose an image file.', imageSizeError: 'Image is too large (max 8MB).',
    loadMore: 'Load more',
  },
  ro: {
    navHome: 'Acasă', navAuctions: 'Licitații', navForum: 'Forum', navAdmin: 'Admin',
    logout: 'Ieși', signIn: 'Autentificare', joinFree: 'Cont gratuit',
    liveNow: 'licitații live acum',
    heroA: 'Licitează simplu.', heroB: 'Câștigă', heroC: 'mare',
    heroSub: 'Marketplace-ul comunității unde fiecare obiect ajunge la cel care oferă cel mai mult. Listează, licitează, câștigă.',
    browseAuctions: 'Vezi licitațiile', sellItem: 'Vinde un obiect',
    statBidsLabel: 'oferte plasate azi', statRatingLabel: 'rating mediu vânzători', statTradedLabel: 'tranzacționați în listări live',
    endingSoon: 'Se termină curând', seeAll: 'Vezi toate licitațiile →', photo: 'foto',
    liveAuctionsTitle: 'Licitații live', searchPh: 'Caută obiecte…',
    sortEnding: 'Se termină curând', sortAsc: 'Preț: crescător', sortDesc: 'Preț: descrescător', sortNew: 'Cele mai noi',
    itemsWord: 'obiecte', bidsWord: 'oferte',
    back: 'Înapoi la licitații', description: 'Descriere', startedAt: 'A pornit de la',
    completedSales: 'vânzări finalizate', viewProfile: 'Vezi profilul',
    currentBid: 'Oferta curentă', startedAtLower: 'a pornit de la', timeLeftLabel: 'Timp rămas', bidsSoFar: 'oferte până acum',
    endedSoldTo: 'Licitație încheiată — vândut lui', endedNoWinner: 'Licitația s-a încheiat fără oferte.',
    placeBid: 'Plasează oferta', bidHistory: 'Istoricul ofertelor',
    noBids: 'Nicio ofertă încă — fii primul!', highest: 'CEA MAI MARE',
    username: 'Nume utilizator', password: 'Parolă', fullName: 'Nume complet',
    memberSince: 'Membru din', reviewsWord: 'recenzii',
    forumTitle: 'Forumul comunității', forumSub: 'Sfaturi, întrebări și povești de la licitatori.',
    postTitlePh: 'Titlul postării…', postBodyPh: 'La ce te gândești?', publish: 'Publică',
    replyPh: 'Scrie un răspuns…', replyBtn: 'Răspunde',
    adminTitle: 'Validare obiecte', pendingWord: 'în așteptare',
    adminSub: 'Listările noi așteaptă aici până le aprobă un admin. Cele respinse nu apar niciodată live.',
    queueEmpty: 'Coada e goală — totul e la zi ✓', listedBy: 'listat de', startsAt: 'pornește de la',
    approve: 'Aprobă', reject: 'Respinge',
    footerNote: 'Proiect demo — Angular + .NET',
    ended: 'Încheiat', minAgo: 'min în urmă', hAgo: 'h în urmă', dAgo: 'z în urmă',
    authTitleReg: 'Creează-ți contul', authTitleLogin: 'Bine ai revenit',
    authSubReg: 'Alătură-te comunității — listează, licitează, câștigă.',
    authSubLogin: 'Autentifică-te ca să licitezi și să-ți administrezi listările.',
    authCtaReg: 'Creează cont', authCtaLogin: 'Autentificare',
    authSwapTextReg: 'Ai deja cont?', authSwapTextLogin: 'Nou pe Bid-App-Igor?',
    authSwapLinkReg: 'Autentificare', authSwapLinkLogin: 'Creează unul',
    fillAll: 'Completează toate câmpurile.',
    signInToBid: 'Autentifică-te ca să plasezi o ofertă.', signInToPost: 'Autentifică-te ca să postezi pe forum.',
    enterBid: 'Introdu o sumă.',
    youHighest: '✓ Ești cel mai mare ofertant cu {p}!',
    inWishlist: '💚 În wishlist', addWishlist: '🤍 Adaugă la wishlist',
    tabMyItems: 'Obiectele mele', tabWon: 'Câștigate', tabWishlist: 'Wishlist', tabReviews: 'Recenzii',
    badgeLive: 'LIVE', badgeWon: 'CÂȘTIGAT 🏆', badgePending: 'ÎN AȘTEPTARE', badgeSold: 'VÂNDUT',
    badgeRejected: 'RESPINS', badgeNoWinner: 'FĂRĂ CÂȘTIGĂTOR', badgeScheduled: 'PROGRAMAT',
    emptyItems: 'Nu ai listări active.', emptyWon: 'Niciun câștig încă — licitează în continuare!',
    emptyWish: 'Wishlist-ul tău e gol. Apasă inima pe orice licitație.', emptyReviews: 'Nicio recenzie încă.',
    hideReplies: 'Ascunde răspunsurile', replies: 'răspunsuri', reply: 'Răspunde',
    approvedMsg: '✓ „{n}" aprobat — e acum live în Licitații.',
    rejectedMsg: '✕ „{n}" respins și scos din coadă.',
    sellTitle: 'Vinde un obiect', sellSub: 'Listarea ta devine live după ce o validează un admin.',
    itemName: 'Numele obiectului', category: 'Categorie', location: 'Locație',
    startPrice: 'Preț de pornire (lei)', duration: 'Durată', days: 'zile',
    submitListing: 'Trimite spre validare',
    sellOk: '✓ Listare trimisă! Un admin o va verifica în curând.',
    yourReview: 'Lasă-i vânzătorului o recenzie', rating: 'Notă', commentPh: 'Cum a fost tranzacția?',
    sendReview: 'Trimite recenzia', reviewOk: '✓ Recenzie trimisă. Mulțumim!',
    winner: 'Câștigător', catAll: 'Toate',
    photoLabel: 'Poză (opțional)', addPhoto: 'Adaugă o poză', changePhoto: 'Schimbă poza', removePhoto: 'Șterge',
    imageTypeError: 'Alege un fișier imagine.', imageSizeError: 'Imaginea e prea mare (max 8MB).',
    loadMore: 'Mai multe',
  },
};

export type Lang = 'en' | 'ro';
export type Dict = (typeof DICTS)['en'];

const CAT_RO: Record<string, string> = {
  All: 'Toate', Electronics: 'Electronice', Art: 'Artă', Auto: 'Auto',
  Collectibles: 'Colecționabile', Fashion: 'Modă',
};

@Injectable({ providedIn: 'root' })
export class I18nService {
  readonly lang = signal<Lang>((localStorage.getItem('lang') as Lang) || 'en');
  readonly t = computed<Dict>(() => DICTS[this.lang()]);

  toggle() {
    const next: Lang = this.lang() === 'en' ? 'ro' : 'en';
    this.lang.set(next);
    localStorage.setItem('lang', next);
  }

  cat(name: string): string {
    return this.lang() === 'ro' ? CAT_RO[name] ?? name : name;
  }

  fmt(n: number): string {
    return n.toLocaleString('ro-RO').replace(/ /g, '.') + ' lei';
  }
}
