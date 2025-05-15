import { Component } from '@angular/core';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-portal',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.css']
})
export class PortalComponent {
  isDropdownOpen = false;

  constructor(public authService: AuthService) {}

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  logout() {
    this.authService.logout();
  }
get canManage(){
  return this.authService.userValue?.roles?.includes('admin') || this.authService.userValue?.roles?.includes('superadmin');
}

}
