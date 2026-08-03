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

  openManuals() {
    window.open('/portal/manuals', '_blank', 'noopener,noreferrer');
  }

  logout() {
    this.authService.logout();
  }
get canManage(){
  return this.authService.isOperationalUser;
}

}
