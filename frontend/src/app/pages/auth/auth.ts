import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { extractErrorMessage } from '../../core/error-utils';
import { I18nService } from '../../core/i18n.service';
import { WishlistService } from '../../core/wishlist.service';

@Component({
  selector: 'app-auth',
  imports: [FormsModule, RouterLink],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})
export class AuthPage {
  readonly i18n = inject(I18nService);
  private readonly auth = inject(AuthService);
  private readonly wishlist = inject(WishlistService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly mode = signal<'login' | 'register'>('login');
  readonly isRegister = computed(() => this.mode() === 'register');

  userName = '';
  name = '';
  email = '';
  password = '';

  readonly error = signal('');
  readonly busy = signal(false);

  private returnUrl = '/auctions';

  constructor() {
    this.route.data.subscribe((data) => {
      this.mode.set(data['mode'] === 'register' ? 'register' : 'login');
      this.error.set('');
    });
    this.route.queryParamMap.subscribe((params) => {
      const url = params.get('returnUrl');
      if (url) this.returnUrl = url;
    });
  }

  submit() {
    this.error.set('');

    if (!this.email || !this.password || (this.isRegister() && !this.userName)) {
      this.error.set(this.i18n.t().fillAll);
      return;
    }

    this.busy.set(true);
    const req = this.isRegister()
      ? this.auth.register(this.userName, this.name, this.email, this.password)
      : this.auth.login(this.email, this.password);

    req.subscribe({
      next: () => {
        this.busy.set(false);
        this.wishlist.load();
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (err) => {
        this.busy.set(false);
        this.error.set(extractErrorMessage(err, this.i18n.t().fillAll));
      },
    });
  }
}
