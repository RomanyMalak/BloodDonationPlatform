import { Component, HostListener } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent {
  isScrolled = false;
  userAvatar = 'assets/images/avatar.png';

  constructor(private authService: AuthService) {}

  @HostListener('window:scroll')
  onScroll() {
    this.isScrolled = window.scrollY > 10;
  }

  getProfileRoute(): string {
  const role = this.authService.getRole();
  if (role === 'Admin') return '/dashboard-Admin';
  if (role === 'Hospital') return '/dashboard/hospital';
  return '/dashboard-User';
}

  isLoggedIn(): boolean {
    return !!this.authService.getRole();
  }
}