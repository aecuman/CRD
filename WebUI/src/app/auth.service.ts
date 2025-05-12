import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginDto, UserDto } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'crd-token';
  private userKey = 'crd-user';
  private tokenExpiryKey = 'crd-token-expiry';


  private userSubject: BehaviorSubject<any>;
  public user: Observable<any>;

  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasValidToken());
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(
      private router: Router,
      private http: HttpClient
  ) {
      this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('user')!));
      this.user = this.userSubject.asObservable();
  }

  public get userValue() {
    return this.userSubject.value;
}
 // Save token with expiry
 saveSession(token: string, user: any): void {
  const expiryTime = new Date().getTime() + 60 * 60 * 1000; // 1 hour from now
  localStorage.setItem(this.tokenKey, token);
  localStorage.setItem(this.userKey, JSON.stringify(user));
  localStorage.setItem(this.tokenExpiryKey, expiryTime.toString());
  this.isLoggedInSubject.next(true);
}

// Clear session
clearSession() {
  localStorage.removeItem(this.tokenKey);
  localStorage.removeItem(this.userKey);
  localStorage.removeItem(this.tokenExpiryKey);
  this.isLoggedInSubject?.next(false);
}


public LoggedInUser(user:LoginDto){
  localStorage.setItem('user', JSON.stringify((user)));
  this.userSubject.next(user)
}
logout(no_redirect?: boolean) {
  localStorage.removeItem(this.userKey);
  localStorage.removeItem(this.tokenKey);
  localStorage.removeItem(this.tokenExpiryKey);
  this.isLoggedInSubject.next(false);
  // Redirect to login page
  this.router.navigate(['/login']);

}


  getToken(): string | null {
    return this.hasValidToken() ? localStorage.getItem(this.tokenKey) : null;
  }

   getUser(): UserDto|null {
    const userJson = localStorage.getItem(this.userKey);
    return userJson ? JSON.parse(userJson) : null;
  }




  isAuthenticated(): boolean {
    return this.hasValidToken();
  }

  private hasValidToken(): boolean {
    const expiryTime = localStorage.getItem(this.tokenExpiryKey);
    if (!expiryTime || new Date().getTime() > parseInt(expiryTime, 10)) {
      this.clearSession();  // Automatically remove expired tokens
      return false;
  }
    const now = new Date().getTime();
    return now < parseInt(expiryTime, 10); // Check if token is still valid
  }


}
