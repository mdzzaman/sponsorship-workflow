import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenResponse } from '../models/auth.model';

let isRefreshing = false;
const accessToken$ = new BehaviorSubject<string | null>(null);

const withBearer = <T extends { clone: (opts: object) => T }>(req: T, token: string): T =>
  req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();
  const outgoing = token ? withBearer(req, token) : req;

  return next(outgoing).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status !== 401 || req.url.includes('/auth/')) {
        return throwError(() => err);
      }

      if (isRefreshing) {
        return accessToken$.pipe(
          filter((t): t is string => t !== null),
          take(1),
          switchMap(t => next(withBearer(req, t)))
        );
      }

      isRefreshing = true;
      accessToken$.next(null);

      return auth.refresh().pipe(
        switchMap((tokens: TokenResponse) => {
          isRefreshing = false;
          accessToken$.next(tokens.accessToken);
          return next(withBearer(req, tokens.accessToken));
        }),
        catchError(refreshErr => {
          isRefreshing = false;
          accessToken$.next(null);
          auth.logoutLocal();
          return throwError(() => refreshErr);
        })
      );
    })
  );
};
