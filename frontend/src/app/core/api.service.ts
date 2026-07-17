import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AuctionItem, Category, ForumPostInfo, ItemDetail, Profile, PublicProfile, ReviewStats,
} from './models';

const API = environment.apiUrl;

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  getItems(): Observable<AuctionItem[]> {
    return this.http.get<AuctionItem[]>(`${API}/item`);
  }

  getItem(id: number): Observable<ItemDetail> {
    return this.http.get<ItemDetail>(`${API}/item/${id}`);
  }

  createItem(dto: {
    name: string; startPrice: number; categoryId: number;
    description?: string; location: string; endDate: string; imageDataUrl?: string;
  }): Observable<{ id: number; status: string }> {
    return this.http.post<{ id: number; status: string }>(`${API}/item`, dto);
  }

  getPendingItems(): Observable<AuctionItem[]> {
    return this.http.get<AuctionItem[]>(`${API}/item/pending`);
  }

  approveItem(id: number): Observable<unknown> {
    return this.http.post(`${API}/item/${id}/approve`, {});
  }

  rejectItem(id: number): Observable<unknown> {
    return this.http.post(`${API}/item/${id}/reject`, {});
  }

  placeBid(itemId: number, amount: number): Observable<{ itemId: number; currentPrice: number; bidCount: number }> {
    return this.http.post<{ itemId: number; currentPrice: number; bidCount: number }>(`${API}/bid`, { itemId, amount });
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${API}/category`);
  }

  getWishlist(): Observable<number[]> {
    return this.http.get<number[]>(`${API}/wishlist`);
  }

  toggleWishlist(itemId: number): Observable<{ itemId: number; inWishlist: boolean }> {
    return this.http.post<{ itemId: number; inWishlist: boolean }>(`${API}/wishlist/${itemId}`, {});
  }

  getProfile(): Observable<Profile> {
    return this.http.get<Profile>(`${API}/user/me`);
  }

  getPublicProfile(id: number): Observable<PublicProfile> {
    return this.http.get<PublicProfile>(`${API}/user/${id}`);
  }

  getReviewStats(): Observable<ReviewStats> {
    return this.http.get<ReviewStats>(`${API}/review/stats`);
  }

  getForumPosts(): Observable<ForumPostInfo[]> {
    return this.http.get<ForumPostInfo[]>(`${API}/forum`);
  }

  addForumPost(title: string, description: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${API}/forum`, { title, description });
  }

  addForumComment(postId: number, text: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${API}/forum/${postId}/comments`, { text });
  }

  addReview(reviewedUserId: number, rating: number, comment: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${API}/review`, { reviewedUserId, rating, comment });
  }
}


