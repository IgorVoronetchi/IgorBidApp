import { Routes } from '@angular/router';
import { HomePage } from './pages/home/home';
import { AuctionsPage } from './pages/auctions/auctions';
import { ItemDetailPage } from './pages/item-detail/item-detail';
import { AuthPage } from './pages/auth/auth';
import { ProfilePage } from './pages/profile/profile';
import { ForumPage } from './pages/forum/forum';
import { AdminPage } from './pages/admin/admin';
import { SellPage } from './pages/sell/sell';
import { PublicProfilePage } from './pages/public-profile/public-profile';
import { adminGuard, authGuard } from './core/guards';

export const routes: Routes = [
  { path: '', component: HomePage },
  { path: 'auctions', component: AuctionsPage },
  { path: 'item/:id', component: ItemDetailPage },
  { path: 'login', component: AuthPage, data: { mode: 'login' } },
  { path: 'register', component: AuthPage, data: { mode: 'register' } },
  { path: 'sell', component: SellPage, canActivate: [authGuard] },
  { path: 'profile', component: ProfilePage, canActivate: [authGuard] },
  { path: 'user/:id', component: PublicProfilePage },
  { path: 'forum', component: ForumPage },
  { path: 'admin', component: AdminPage, canActivate: [adminGuard] },
  { path: '**', redirectTo: '' },
];
