import { Component, HostListener, OnInit, OnDestroy, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService, NotificationDto } from '../../../core/services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  isScrolled = false;
  userAvatar = 'assets/images/avatar.png';
  showNotifications = false;
  mobileMenuOpen = false;

  notifications: NotificationDto[] = [];
  unreadCount = 0;

  private subs: Subscription[] = [];

  ngOnInit() {
    if (this.isLoggedIn()) {
      this.notificationService.startConnection();
      this.notificationService.loadNotifications();

      this.subs.push(
        this.notificationService.notifications$.subscribe(n => this.notifications = n),
        this.notificationService.unreadCount$.subscribe(c => this.unreadCount = c)
      );
    }
  }

  ngOnDestroy() {
    this.subs.forEach(s => s.unsubscribe());
    this.notificationService.stopConnection();
  }

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

  logout(): void {
    this.authService.logout();
    this.notificationService.stopConnection();
    this.router.navigate(['/']);
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
  }

  markAllRead() {
    this.notificationService.markAllAsRead().subscribe({
      next: () => this.notificationService.loadNotifications(),
      error: (err) => console.error(err)
    });
  }

  markRead(id: string) {
    this.notificationService.markAsRead(id).subscribe({
      next: () => this.notificationService.loadNotifications(),
      error: (err) => console.error(err)
    });
  }
}