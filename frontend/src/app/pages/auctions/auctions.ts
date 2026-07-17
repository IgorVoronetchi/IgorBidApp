import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AuthService } from '../../core/auth.service';
import { I18nService } from '../../core/i18n.service';
import { TickerService } from '../../core/ticker.service';
import { WishlistService } from '../../core/wishlist.service';
import { AuctionItem, Category } from '../../core/models';

type SortKey = 'ending' | 'priceAsc' | 'priceDesc' | 'newest';

const PAGE_SIZE = 6;

@Component({
  selector: 'app-auctions',
  imports: [FormsModule, RouterLink],
  templateUrl: './auctions.html',
  styleUrl: './auctions.css',
})
export class AuctionsPage {
  readonly i18n = inject(I18nService);
  readonly ticker = inject(TickerService);
  readonly wishlist = inject(WishlistService);
  readonly auth = inject(AuthService);
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  readonly items = signal<AuctionItem[]>([]);
  readonly categories = signal<Category[]>([]);

  q = '';
  readonly qSignal = signal('');
  readonly cat = signal('All');
  readonly sort = signal<SortKey>('ending');
  readonly visibleCount = signal(PAGE_SIZE);

  readonly filtered = computed(() => {
    let list = this.items().filter((i) => i.status === 'ActiveBid');
    const q = this.qSignal().trim().toLowerCase();
    if (q) list = list.filter((i) => i.name.toLowerCase().includes(q));
    if (this.cat() !== 'All') list = list.filter((i) => i.category === this.cat());
    switch (this.sort()) {
      case 'priceAsc': list = [...list].sort((a, b) => a.currentPrice - b.currentPrice); break;
      case 'priceDesc': list = [...list].sort((a, b) => b.currentPrice - a.currentPrice); break;
      case 'newest': list = [...list].sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime()); break;
      default: list = [...list].sort((a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime());
    }
    return list;
  });

  readonly paged = computed(() => this.filtered().slice(0, this.visibleCount()));
  readonly hasMore = computed(() => this.visibleCount() < this.filtered().length);

  constructor() {
    this.api.getItems().subscribe((items) => this.items.set(items));
    this.api.getCategories().subscribe((cats) => this.categories.set(cats));
    this.wishlist.load();
  }

  onSearch(value: string) {
    this.qSignal.set(value);
    this.visibleCount.set(PAGE_SIZE);
  }

  onSort(value: string) {
    this.sort.set(value as SortKey);
    this.visibleCount.set(PAGE_SIZE);
  }

  selectCat(name: string) {
    this.cat.set(name);
    this.visibleCount.set(PAGE_SIZE);
  }

  loadMore() {
    this.visibleCount.update((n) => n + PAGE_SIZE);
  }

  toggleWish(event: Event, itemId: number) {
    event.preventDefault();
    event.stopPropagation();
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/auctions' } });
      return;
    }
    this.wishlist.toggle(itemId);
  }
}
