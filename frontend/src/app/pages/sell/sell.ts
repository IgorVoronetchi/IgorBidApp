import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { extractErrorMessage } from '../../core/error-utils';
import { I18nService } from '../../core/i18n.service';
import { resizeImageToDataUrl } from '../../core/image-utils';
import { Category } from '../../core/models';

const MAX_SOURCE_BYTES = 8 * 1024 * 1024; // 8MB, inainte de redimensionare

@Component({
  selector: 'app-sell',
  imports: [FormsModule],
  templateUrl: './sell.html',
  styleUrl: './sell.css',
})
export class SellPage {
  readonly i18n = inject(I18nService);
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  readonly categories = signal<Category[]>([]);

  name = '';
  categoryId: number | null = null;
  description = '';
  location = '';
  startPrice: number | null = null;
  durationDays = 3;

  readonly imagePreview = signal<string | null>(null);
  readonly imageBusy = signal(false);
  readonly imageError = signal('');

  readonly error = signal('');
  readonly done = signal(false);
  readonly busy = signal(false);

  constructor() {
    this.api.getCategories().subscribe((cats) => {
      this.categories.set(cats);
      if (cats.length > 0) this.categoryId = cats[0].id;
    });
  }

  async onFileSelected(event: Event) {
    this.imageError.set('');
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.imageError.set(this.i18n.t().imageTypeError);
      return;
    }
    if (file.size > MAX_SOURCE_BYTES) {
      this.imageError.set(this.i18n.t().imageSizeError);
      return;
    }

    this.imageBusy.set(true);
    try {
      const dataUrl = await resizeImageToDataUrl(file);
      this.imagePreview.set(dataUrl);
    } catch {
      this.imageError.set(this.i18n.t().imageTypeError);
    } finally {
      this.imageBusy.set(false);
    }
  }

  removeImage() {
    this.imagePreview.set(null);
    this.imageError.set('');
  }

  submit() {
    this.error.set('');

    if (!this.name || !this.location || !this.startPrice || this.startPrice <= 0 || !this.categoryId) {
      this.error.set(this.i18n.t().fillAll);
      return;
    }

    const endDate = new Date(Date.now() + this.durationDays * 86400000).toISOString();

    this.busy.set(true);
    this.api
      .createItem({
        name: this.name,
        startPrice: this.startPrice,
        categoryId: this.categoryId,
        description: this.description,
        location: this.location,
        endDate,
        imageDataUrl: this.imagePreview() ?? undefined,
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.done.set(true);
          setTimeout(() => this.router.navigate(['/profile']), 1800);
        },
        error: (err) => {
          this.busy.set(false);
          this.error.set(extractErrorMessage(err, this.i18n.t().fillAll));
        },
      });
  }
}
