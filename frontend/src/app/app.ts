import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { I18nService } from './core/i18n.service';
import { WishlistService } from './core/wishlist.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  readonly auth = inject(AuthService);
  readonly i18n = inject(I18nService);
  private readonly wishlist = inject(WishlistService);
  private readonly router = inject(Router);

  readonly initials = computed(() => {
    const u = this.auth.user();
    if (!u) return '';
    const parts = (u.name || u.userName).trim().split(/\s+/);
    return parts.slice(0, 2).map((p) => p[0]?.toUpperCase() ?? '').join('');
  });

  readonly langLabel = computed(() => (this.i18n.lang() === 'en' ? 'EN' : 'RO'));

  constructor() {
    this.wishlist.load();
  }

  logout() {
    this.auth.logout();
    this.wishlist.load();
    this.router.navigate(['/']);
  }
}
