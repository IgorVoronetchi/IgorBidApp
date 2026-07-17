import { Injectable, signal } from '@angular/core';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  readonly ids = signal<ReadonlySet<number>>(new Set());

  constructor(private api: ApiService, private auth: AuthService) {}

  load() {
    if (!this.auth.isLoggedIn()) {
      this.ids.set(new Set());
      return;
    }
    this.api.getWishlist().subscribe({
      next: (ids) => this.ids.set(new Set(ids)),
      error: () => this.ids.set(new Set()),
    });
  }

  has(itemId: number): boolean {
    return this.ids().has(itemId);
  }

  toggle(itemId: number) {
    this.api.toggleWishlist(itemId).subscribe((res) => {
      const next = new Set(this.ids());
      if (res.inWishlist) next.add(itemId);
      else next.delete(itemId);
      this.ids.set(next);
    });
  }
}
