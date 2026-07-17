export interface AuctionItem {
  id: number;
  name: string;
  startPrice: number;
  currentPrice: number;
  category: string;
  categoryId: number;
  description?: string;
  location: string;
  imageUrl?: string;
  ownerId: number;
  ownerUserName: string;
  winnerId?: number;
  winnerUserName?: string;
  status: string;
  startDate: string;
  endDate: string;
  bidCount: number;
}

export interface BidInfo {
  id: number;
  bidderId: number;
  bidderUserName: string;
  amount: number;
  placedAt: string;
}

export interface ItemDetail extends AuctionItem {
  ownerRating?: number;
  ownerSales: number;
  bids: BidInfo[];
}

export interface SessionUser {
  id: number;
  userName: string;
  name: string;
  email: string;
  role: string;
}

export interface Category {
  id: number;
  name: string;
}

export interface ReviewInfo {
  id: number;
  reviewerUserName: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface Profile {
  id: number;
  userName: string;
  name: string;
  email: string;
  role: string;
  memberSince: string;
  rating?: number;
  addedItems: AuctionItem[];
  wonItems: AuctionItem[];
  wishlistItems: AuctionItem[];
  reviews: ReviewInfo[];
}

export interface ForumCommentInfo {
  id: number;
  authorUserName: string;
  text: string;
  createdAt: string;
}

export interface ForumPostInfo {
  id: number;
  title: string;
  description: string;
  authorUserName: string;
  createdAt: string;
  comments: ForumCommentInfo[];
}

export interface PublicProfile {
  id: number;
  userName: string;
  name: string;
  memberSince: string;
  rating?: number;
  completedSales: number;
  reviews: ReviewInfo[];
}

export interface ReviewStats {
  avgRating?: number;
  count: number;
}
