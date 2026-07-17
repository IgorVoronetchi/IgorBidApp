import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.token;

  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  const isAuthEndpoint = req.url.includes('/user/login') || req.url.includes('/user/register');

  return next(req).pipe(
    catchError((err: unknown) => {
      // Un token expirat/invalid pe o cerere autentificata -> delogare + redirect la login.
      // Exceptie: raspunsul 401 de la login (parola gresita) nu inseamna sesiune expirata.
      if (token && !isAuthEndpoint && err instanceof HttpErrorResponse && err.status === 401) {
        auth.logout();
        router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
      }
      return throwError(() => err);
    }),
  );
};
