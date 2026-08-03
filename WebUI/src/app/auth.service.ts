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
      this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem(this.userKey)!));
      this.user = this.userSubject.asObservable();
  }

  public get userValue():UserDto {
    return this.userSubject.value;
}
 // Save token with expiry
 saveSession(token: string, user: any): void {
  const expiryTime = new Date().getTime() + 60 * 60 * 1000; // 1 hour from now
  localStorage.setItem(this.tokenKey, token);
  localStorage.setItem(this.userKey, JSON.stringify(user));
  localStorage.setItem(this.tokenExpiryKey, expiryTime.toString());
  this.userSubject.next(user);
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

  get getUser(): UserDto|null {
    const userJson = localStorage.getItem(this.userKey);
    return userJson ? JSON.parse(userJson) : null;
  }




  isAuthenticated(): boolean {
    return this.hasValidToken();
  }

  // True for any user who can perform workflow actions (not read-only).
  // Matches roles defined in WorkflowSeed: Registry, DataInputClerk,
  // ChairModerationCommittee, ModerationCommitteeSecretary, RegionalOfficer (Manager), admin.
  get isOperationalUser(): boolean {
    const operationalRoles = [
      'admin', 'superadmin',
      'Manager', 'RegionalOfficer',
      'Registry',
      'ChairModerationCommittee',
      'DataInputClerk',
      'ModerationCommitteeSecretary'
    ];
    return this.userValue?.roles?.some((r: string) => operationalRoles.includes(r)) ?? false;
  }

  get isAdmin(): boolean {
    return this.userValue?.roles?.some((r: string) => r === 'admin' || r === 'superadmin') ?? false;
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
