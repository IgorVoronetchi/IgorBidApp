import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { I18nService } from '../../core/i18n.service';
import { TickerService } from '../../core/ticker.service';
import { AuctionItem } from '../../core/models';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomePage {
  readonly i18n = inject(I18nService);
  readonly ticker = inject(TickerService);
  private readonly api = inject(ApiService);

  readonly items = signal<AuctionItem[]>([]);
  readonly avgRating = signal<number | null>(null);

  readonly liveItems = computed(() =>
    this.items().filter((i) => i.status === 'ActiveBid'),
  );

  readonly featured = computed(() =>
    [...this.liveItems()]
      .sort((a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime())
      .slice(0, 3),
  );

  readonly totalBids = computed(() =>
    this.items().reduce((sum, i) => sum + i.bidCount, 0),
  );

  readonly totalTraded = computed(() =>
    this.liveItems().reduce((sum, i) => sum + i.currentPrice, 0),
  );

  constructor() {
    this.api.getItems().subscribe((items) => this.items.set(items));
    this.api.getReviewStats().subscribe((stats) => this.avgRating.set(stats.avgRating ?? null));
  }
}
