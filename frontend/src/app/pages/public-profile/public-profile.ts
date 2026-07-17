import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { I18nService } from '../../core/i18n.service';
import { PublicProfile } from '../../core/models';

@Component({
  selector: 'app-public-profile',
  imports: [],
  templateUrl: './public-profile.html',
  styleUrl: './public-profile.css',
})
export class PublicProfilePage {
  readonly i18n = inject(I18nService);
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly profile = signal<PublicProfile | null>(null);
  readonly notFound = signal(false);

  readonly initials = computed(() => {
    const p = this.profile();
    if (!p) return '';
    const parts = (p.name || p.userName).trim().split(/\s+/);
    return parts.slice(0, 2).map((x) => x[0]?.toUpperCase() ?? '').join('');
  });

  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (id) this.load(id);
    });
  }

  private load(id: number) {
    this.profile.set(null);
    this.notFound.set(false);
    this.api.getPublicProfile(id).subscribe({
      next: (p) => this.profile.set(p),
      error: () => this.notFound.set(true),
    });
  }

  back() {
    this.router.navigate(['/auctions']);
  }

  stars(n: number): string {
    return '★'.repeat(n) + '☆'.repeat(5 - n);
  }

  memberSinceLabel(): string {
    const p = this.profile();
    if (!p) return '';
    return new Date(p.memberSince).toLocaleDateString(
      this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB',
      { month: 'long', year: 'numeric' },
    );
  }

  fmtDate(d: string): string {
    return new Date(d).toLocaleDateString(this.i18n.lang() === 'ro' ? 'ro-RO' : 'en-GB', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }
}
