import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { CurrentUser, LoginRequest, LoginResponse, TokenResponse } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly ACCESS_TOKEN_KEY = 'sw_access_token';
  private readonly REFRESH_TOKEN_KEY = 'sw_refresh_token';
  private readonly USER_KEY = 'sw_user';

  currentUser = signal<CurrentUser | null>(this.loadUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(res => {
        const user: CurrentUser = {
          userId: res.userId,
          email: res.email,
          fullName: res.fullName,
          roles: res.roles,
          accessToken: res.accessToken,
          refreshToken: res.refreshToken
        };
        this.persistSession(user);
      })
    );
  }

  refresh() {
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
    return this.http.post<TokenResponse>(`${environment.apiUrl}/auth/refresh`, { refreshToken }).pipe(
      tap(res => {
        const user = this.currentUser();
        if (user) {
          const updated: CurrentUser = { ...user, accessToken: res.accessToken, refreshToken: res.refreshToken };
          this.persistSession(updated);
        }
      })
    );
  }

  logout() {
    this.http.post(`${environment.apiUrl}/auth/logout`, {}).subscribe({ error: () => {} });
    this.clearSession();
  }

  logoutLocal() {
    this.clearSession();
  }

  getToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    return this.currentUser()?.roles.includes(role) ?? false;
  }

  hasAnyRole(roles: string[]): boolean {
    const userRoles = this.currentUser()?.roles ?? [];
    return roles.some(r => userRoles.includes(r));
  }

  private persistSession(user: CurrentUser) {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, user.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, user.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private clearSession() {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  private loadUser(): CurrentUser | null {
    try {
      const raw = localStorage.getItem(this.USER_KEY);
      return raw ? (JSON.parse(raw) as CurrentUser) : null;
    } catch {
      return null;
    }
  }
}
