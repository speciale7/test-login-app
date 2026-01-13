import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError, tap, catchError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import {
  User,
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  RefreshTokenRequest
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user';

  // Using signals for reactive state management
  public currentUser = signal<User | null>(null);
  public isAuthenticated = signal<boolean>(false);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromStorage();
  }

  /**
   * Load user from localStorage on service initialization
   */
  private loadUserFromStorage(): void {
    const token = this.getAccessToken();
    const userJson = localStorage.getItem(this.USER_KEY);

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson);
        this.currentUser.set(user);
        this.isAuthenticated.set(true);
      } catch (error) {
        console.error('Error parsing user data:', error);
        this.clearAuthData();
      }
    }
  }

  /**
   * Register a new user
   */
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/register`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(this.handleError)
      );
  }

  /**
   * Login user
   */
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/login`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(this.handleError)
      );
  }

  /**
   * Refresh access token
   */
  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    const request: RefreshTokenRequest = { refreshToken };
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/refresh`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(error => {
          this.logout();
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current user info from API
   */
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/auth/me`)
      .pipe(
        tap(user => {
          this.currentUser.set(user);
          localStorage.setItem(this.USER_KEY, JSON.stringify(user));
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Logout user
   */
  logout(): void {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      const request: RefreshTokenRequest = { refreshToken };
      this.http.post(`${this.API_URL}/auth/logout`, request)
        .subscribe({
          error: (error) => console.error('Logout error:', error)
        });
    }

    this.clearAuthData();
    this.router.navigate(['/login']);
  }

  /**
   * Handle successful authentication response
   */
  private handleAuthResponse(response: AuthResponse): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));

    this.currentUser.set(response.user);
    this.isAuthenticated.set(true);
  }

  /**
   * Clear all authentication data
   */
  private clearAuthData(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);

    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  /**
   * Get access token from storage
   */
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get refresh token from storage
   */
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Check if current user can write (Writer or Admin)
   */
  canWrite(): boolean {
    const user = this.currentUser();
    return user !== null && (user.role === 1 || user.role === 2); // Writer or Admin
  }

  /**
   * Check if current user is admin
   */
  isAdmin(): boolean {
    const user = this.currentUser();
    return user !== null && user.role === 2; // Admin
  }

  /**
   * Check if current user is reader only
   */
  isReader(): boolean {
    const user = this.currentUser();
    return user !== null && user.role === 0; // Reader
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = error.error?.message || `Server returned code ${error.status}`;
    }

    return throwError(() => new Error(errorMessage));
  }
}
