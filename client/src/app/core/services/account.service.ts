import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  isAdmin = computed(() => {
    const token = !!localStorage.getItem('token');

    return !!token;
  });

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http
      .post<User>(this.baseUrl + 'account/login', values, {
        params,
      })
      .pipe(
        map((response) => {
          console.log(response);
          localStorage.setItem('token', response.token);
          localStorage.setItem('user', JSON.stringify(response));
          return response;
        })
      );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  getUserInfo() {
    const token = localStorage.getItem('token');
    const headers = token
      ? new HttpHeaders().set('Authorization', `Bearer ${token}`)
      : new HttpHeaders();

    return this.http
      .get<User>(this.baseUrl + 'account/user-info', { headers })
      .pipe(
        map((user) => {
          this.currentUser.set(user);
          return user;
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUser.set(null);
    window.location.reload();
    window.location.href = '/portal';
    return this.http.post(this.baseUrl + 'account/logout', {});
  }

  getAuthState() {
    return this.http.get<{ isAuthenticated: boolean }>(
      this.baseUrl + 'account/auth-status'
    );
  }
}
