import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDto, LoginDto, AuthResponse, RegisterDto, ChangePasswordDto } from '../../shared/models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = `${environment.apiUrl}/auth`;

  currentUser = signal<UserDto | null>(this.loadUser());
  isLoggedIn  = signal<boolean>(!!this.getToken());

  constructor(private http: HttpClient, private router: Router) {}

  login(dto: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/login`, dto).pipe(
      tap(res => { if (res.isSuccess) this.saveSession(res); })
    );
  }

  register(dto: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/register`, dto).pipe(
      tap(res => { if (res.isSuccess) this.saveSession(res); })
    );
  }

  logout(): void {
    this.http.post(`${this.api}/logout`, {}).subscribe();
    this.clearSession();
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = localStorage.getItem('refreshToken') ?? '';
    return this.http.post<AuthResponse>(`${this.api}/refresh-token`, { refreshToken }).pipe(
      tap(res => { if (res.isSuccess) this.saveSession(res); })
    );
  }

  changePassword(dto: ChangePasswordDto): Observable<any> {
    return this.http.post(`${this.api}/change-password`, dto);
  }

  getToken(): string | null { return localStorage.getItem('token'); }

  isAdmin(): boolean { return this.currentUser()?.role === 'Admin'; }

  private saveSession(res: AuthResponse): void {
    localStorage.setItem('token', res.token!);
    localStorage.setItem('refreshToken', res.refreshToken!);
    localStorage.setItem('user', JSON.stringify(res.user));
    this.currentUser.set(res.user!);
    this.isLoggedIn.set(true);
  }

  private clearSession(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.isLoggedIn.set(false);
  }

  private loadUser(): UserDto | null {
    const u = localStorage.getItem('user');
    return u ? JSON.parse(u) : null;
  }
}
