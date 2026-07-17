import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AuthService } from '../../core/auth.service';
import { extractErrorMessage } from '../../core/error-utils';
import { I18nService } from '../../core/i18n.service';
import { TickerService } from '../../core/ticker.service';
import { WishlistService } from '../../core/wishlist.service';
import { ItemDetail } from '../../core/models';

@Component({
  selector: 'app-item-detail',
  imports: [FormsModule, RouterLink],
  templateUrl: './item-detail.html',
  styleUrl: './item-detail.css',
})
export class ItemDetailPage {
  readonly i18n = inject(I18nService);
  readonly ticker = inject(TickerService);
  readonly auth = inject(AuthService);
  readonly wishlist = inject(WishlistService);
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private pollHandle: ReturnType<typeof setInterval> | null = null;

  readonly item = signal<ItemDetail | null>(null);

  bidValue = '';
  readonly bidError = signal('');
  readonly bidOk = signal('');
  readonly busy = signal(false);

  reviewRating = 5;
  reviewComment = '';
  readonly reviewSent = signal(false);
  readonly reviewError = signal('');

  readonly isEnded = computed(() => {
    const it = this.item();
    if (!it) return false;
    if (it.status === 'Sold' || it.status === 'NoWinner') return true;
    return new Date(it.endDate).getTime() <= this.ticker.now();
  });

  readonly isOwner = computed(() => this.item()?.ownerId === this.auth.user()?.id);

  readonly isWinner = computed(() => {
    const it = this.item();
    const u = this.auth.user();
    return !!it && !!u && it.status === 'Sold' && it.winnerId === u.id;
  });

  readonly topBidder = computed(() => this.item()?.bids[0]?.bidderUserName ?? this.item()?.winnerUserName ?? '');

  readonly increments = computed(() => {
    const price = this.item()?.currentPrice ?? 0;
    if (price >= 10000) return [500, 1000, 2500];
    if (price >= 1000) return [50, 100, 250];
    return [10, 25, 50];
  });

  readonly minNextBid = computed(() => {
    const it = this.item();
    if (!it) return 0;
    return it.bidCount === 0 ? it.startPrice : it.currentPrice + 1;
  });

  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (id) this.load(id);
    });
    this.destroyRef.onDestroy(() => this.stopPolling());
  }

  private load(id: number) {
    this.stopPolling();
    this.api.getItem(id).subscribe({
      next: (item) => {
        this.item.set(item);
        this.startPolling(id);
      },
      error: () => this.router.navigate(['/auctions']),
    });
  }

  /** Refresh silențios la fiecare 5s cât timp licitația e activă, ca ofertele altor useri să apară fără reload manual. */
  private startPolling(id: number) {
    this.pollHandle = setInterval(() => {
      if (this.isEnded()) {
        this.stopPolling();
        return;
      }
      this.api.getItem(id).subscribe({
        next: (item) => this.item.set(item),
        error: () => this.stopPolling(),
      });
    }, 5000);
  }

  private stopPolling() {
    if (this.pollHandle !== null) {
      clearInterval(this.pollHandle);
      this.pollHandle = null;
    }
  }

  ownerInitial(): string {
    return (this.item()?.ownerUserName[0] ?? '?').toUpperCase();
  }

  quickBid(inc: number) {
    const it = this.item();
    if (!it) return;
    const base = it.bidCount === 0 ? it.startPrice : it.currentPrice;
    this.bidValue = String(base + inc);
  }

  placeBid() {
    this.bidError.set('');
    this.bidOk.set('');

    if (!this.auth.isLoggedIn()) {
      this.bidError.set(this.i18n.t().signInToBid);
      return;
    }

    const it = this.item();
    if (!it) return;

    const amount = Number(this.bidValue);
    if (!this.bidValue || isNaN(amount) || amount <= 0) {
      this.bidError.set(this.i18n.t().enterBid);
      return;
    }

    this.busy.set(true);
    this.api.placeBid(it.id, amount).subscribe({
      next: () => {
        this.busy.set(false);
        this.bidValue = '';
        this.bidOk.set(this.i18n.t().youHighest.replace('{p}', this.i18n.fmt(amount)));
        this.load(it.id);
      },
      error: (err) => {
        this.busy.set(false);
        this.bidError.set(extractErrorMessage(err, this.i18n.t().enterBid));
      },
    });
  }

  toggleWish() {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }
    const it = this.item();
    if (it) this.wishlist.toggle(it.id);
  }

  sendReview() {
    const it = this.item();
    if (!it) return;
    this.reviewError.set('');
    this.api.addReview(it.ownerId, this.reviewRating, this.reviewComment).subscribe({
      next: () => this.reviewSent.set(true),
      error: (err) =>
        this.reviewError.set(extractErrorMessage(err, 'Error')),
    });
  }

  fmtDate(d: string): string {
    return new Date(d).toLocaleDateString(this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }
}
