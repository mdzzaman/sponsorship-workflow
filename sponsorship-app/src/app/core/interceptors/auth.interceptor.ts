import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

let isRefreshing = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      const isAuthEndpoint = req.url.includes('/auth/refresh') || req.url.includes('/auth/login');
      if (err.status === 401 && !isAuthEndpoint && !isRefreshing) {
        isRefreshing = true;
        return auth.refresh().pipe(
          switchMap(tokens => {
            isRefreshing = false;
            const retried = req.clone({ setHeaders: { Authorization: `Bearer ${tokens.accessToken}` } });
            return next(retried);
          }),
          catchError(refreshErr => {
            isRefreshing = false;
            auth.logoutLocal();
            return throwError(() => refreshErr);
          })
        );
      }
      return throwError(() => err);
    })
  );
};
