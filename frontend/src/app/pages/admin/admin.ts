import { Component, inject, signal } from '@angular/core';
import { ApiService } from '../../core/api.service';
import { I18nService } from '../../core/i18n.service';
import { AuctionItem } from '../../core/models';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class AdminPage {
  readonly i18n = inject(I18nService);
  private readonly api = inject(ApiService);

  readonly pending = signal<AuctionItem[]>([]);
  readonly message = signal('');

  constructor() {
    this.load();
  }

  private load() {
    this.api.getPendingItems().subscribe((items) => this.pending.set(items));
  }

  approve(item: AuctionItem) {
    this.api.approveItem(item.id).subscribe(() => {
      this.message.set(this.i18n.t().approvedMsg.replace('{n}', item.name));
      this.load();
    });
  }

  reject(item: AuctionItem) {
    this.api.rejectItem(item.id).subscribe(() => {
      this.message.set(this.i18n.t().rejectedMsg.replace('{n}', item.name));
      this.load();
    });
  }

  fmtDate(d: string): string {
    return new Date(d).toLocaleDateString(this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB', {
      day: 'numeric', month: 'short',
    });
  }
}
