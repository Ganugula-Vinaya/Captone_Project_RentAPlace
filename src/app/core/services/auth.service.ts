import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthResponse, LoginDto, RegisterDto } from '../../models/auth.models';
import { BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = `${environment.apiBase}/api/auth`;
  private currentUserSub = new BehaviorSubject<AuthResponse | null>(this.getStored());
  currentUser$ = this.currentUserSub.asObservable();

  constructor(private http: HttpClient) {}

  // Register new user
  register(dto: RegisterDto) {
    return this.http.post<AuthResponse>(`${this.api}/register`, dto)
      .pipe(tap(res => this.setUser(res)));
  }

  // Login
  login(dto: LoginDto) {
    return this.http.post<AuthResponse>(`${this.api}/login`, dto)
      .pipe(tap(res => this.setUser(res)));
  }

  logout() {
    localStorage.removeItem('auth');
    this.currentUserSub.next(null);
  }

  get token(): string | null {
    return this.currentUserSub.value?.token ?? null;
  }

  get role(): 'Owner' | 'Renter' | null {
    return this.currentUserSub.value?.role ?? null;
  }

  get userId(): string | null {
    return this.currentUserSub.value?.id?.toString() ?? null; // <-- assuming AuthResponse has 'id' property
  }

  private setUser(res: AuthResponse) {
    localStorage.setItem('auth', JSON.stringify(res));
    this.currentUserSub.next(res);
  }

  private getStored(): AuthResponse | null {
    const s = localStorage.getItem('auth');
    return s ? JSON.parse(s) as AuthResponse : null;
  }
}
