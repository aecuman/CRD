import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginDto } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject: BehaviorSubject<any>;
  public user: Observable<any>;

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
public LoggedInUser(user:LoginDto){
  localStorage.setItem('user', JSON.stringify((user)));
  this.userSubject.next(user)
}
  logout(no_redirect?:boolean) {
    localStorage.removeItem('user');
  }
}
