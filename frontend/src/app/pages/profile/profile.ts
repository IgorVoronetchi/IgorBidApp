import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { I18nService } from '../../core/i18n.service';
import { TickerService } from '../../core/ticker.service';
import { AuctionItem, Profile } from '../../core/models';

type Tab = 'items' | 'won' | 'wishlist' | 'reviews';

@Component({
  selector: 'app-profile',
  imports: [RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfilePage {
  readonly i18n = inject(I18nService);
  readonly ticker = inject(TickerService);
  private readonly api = inject(ApiService);

  readonly profile = signal<Profile | null>(null);
  readonly tab = signal<Tab>('items');

  readonly tabs: { key: Tab }[] = [
    { key: 'items' }, { key: 'won' }, { key: 'wishlist' }, { key: 'reviews' },
  ];

  readonly tabItems = computed<AuctionItem[]>(() => {
    const p = this.profile();
    if (!p) return [];
    switch (this.tab()) {
      case 'items': return p.addedItems;
      case 'won': return p.wonItems;
      case 'wishlist': return p.wishlistItems;
      default: return [];
    }
  });

  readonly initials = computed(() => {
    const p = this.profile();
    if (!p) return '';
    const parts = (p.name || p.userName).trim().split(/\s+/);
    return parts.slice(0, 2).map((x) => x[0]?.toUpperCase() ?? '').join('');
  });

  constructor() {
    this.api.getProfile().subscribe((p) => this.profile.set(p));
  }

  tabLabel(key: Tab): string {
    const t = this.i18n.t();
    return { items: t.tabMyItems, won: t.tabWon, wishlist: t.tabWishlist, reviews: t.tabReviews }[key];
  }

  emptyText(): string {
    const t = this.i18n.t();
    switch (this.tab()) {
      case 'items': return t.emptyItems;
      case 'won': return t.emptyWon;
      case 'wishlist': return t.emptyWish;
      default: return t.emptyReviews;
    }
  }

  badge(it: AuctionItem): { label: string; cls: string } {
    const t = this.i18n.t();
    if (this.tab() === 'won') return { label: t.badgeWon, cls: 'won' };
    switch (it.status) {
      case 'ActiveBid': return { label: t.badgeLive, cls: 'live' };
      case 'Added': return { label: t.badgePending, cls: 'pending' };
      case 'Validated': return { label: t.badgeScheduled, cls: 'pending' };
      case 'Sold': return { label: t.badgeSold, cls: 'won' };
      case 'Rejected': return { label: t.badgeRejected, cls: 'rejected' };
      default: return { label: t.badgeNoWinner, cls: 'rejected' };
    }
  }

  memberSinceLabel(): string {
    const p = this.profile();
    if (!p) return '';
    return new Date(p.memberSince).toLocaleDateString(
      this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB',
      { month: 'long', year: 'numeric' },
    );
  }

  stars(n: number): string {
    return '★'.repeat(n) + '☆'.repeat(5 - n);
  }

  fmtDate(d: string): string {
    return new Date(d).toLocaleDateString(this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }
}
