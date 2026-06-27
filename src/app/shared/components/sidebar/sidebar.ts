import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StorageService } from '../../../core/services/storage.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent {
  private router = inject(Router);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private storage = inject(StorageService);

  goToAiPipeline() {
    const savedRequest = this.storage.get('lastRequest');
    const request = savedRequest ? JSON.parse(savedRequest) : null;

    this.router.navigate(['/AiPipeline'], {
      state: { request }
    });
  }

  logout() {
    this.authService.logout();
    this.notificationService.stopConnection();
    this.router.navigate(['/']);
  }
}