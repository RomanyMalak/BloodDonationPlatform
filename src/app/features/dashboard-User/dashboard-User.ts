import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { DonorService } from '../../core/services/donor.service';
import { BloodRequestSummaryDto, DonorNearbyRequestDto, BloodRequestDetailsDto } from '../../shared/models/blood-request.model';
import { StorageService } from '../../core/services/storage.service';
import { NotificationDto, NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-dashboard-user',
  standalone: true,
  imports: [CommonModule, RouterLink, SidebarComponent],
  templateUrl: './dashboard-User.html',
  styleUrl: './dashboard-User.css'
})
export class DashboardUser implements OnInit {

  private bloodRequestService = inject(BloodRequestService);
  private donorService = inject(DonorService);
  private storage = inject(StorageService);
  private notificationService = inject(NotificationService);
  private route = inject(ActivatedRoute);

  notifications: NotificationDto[] = [];
  fullName = '';
  isAvailable = true;
  isLoading = false;

  availableRequests: BloodRequestSummaryDto[] = [];
  myRequests: BloodRequestSummaryDto[] = [];
  nearbyRequests: DonorNearbyRequestDto[] = [];
  donationHistory: any[] = [];

  // ===== Donate Modal =====
  showDonateModal = false;
  selectedRequest: BloodRequestDetailsDto | null = null;
  showSuccessAlert = false;
  successMessage = '';

  ngOnInit() {
    this.fullName = this.storage.get('fullName') || 'مستخدم';
    this.loadData();

    // لو جه من إشعار
   this.route.queryParams.subscribe(params => {
  console.log('query params:', params);
  const requestId = params['donateRequestId'];
  if (requestId) {
    this.openDonateModal(requestId);
  }
});
  }

openDonateModal(requestId: string) {
    this.bloodRequestService.getById(requestId).subscribe({
      next: (res) => {
        this.selectedRequest = res;
        this.showDonateModal = true;
      },
      error: (err) => console.error('load request error:', err)
    });
  }

  confirmDonate() {
    if (!this.selectedRequest) return;

    this.bloodRequestService.accept(this.selectedRequest.id).subscribe({
      next: () => {
        this.showDonateModal = false;
        this.selectedRequest = null;
        this.successMessage = 'تم تأكيد طلب التبرع بنجاح';
        this.showSuccessAlert = true;
        this.loadData();

        setTimeout(() => {
          this.showSuccessAlert = false;
          this.successMessage = '';
        }, 3000);
      },
      error: (err) => {
        console.error('confirm donate error:', err);
        this.successMessage = 'حدث خطأ أثناء تأكيد طلب التبرع';
        this.showSuccessAlert = true;
      }
    });
  }

  closeDonateModal() {
    this.showDonateModal = false;
    this.selectedRequest = null;
  }

  loadData() {
    const token = this.storage.get('token');
    if (!token) return;

    this.isLoading = true;

    this.bloodRequestService.getAvailable().subscribe({
      next: (res) => {
        this.availableRequests = res;
        this.isLoading = false;
        this.notificationService.loadNotifications();
        this.notificationService.notifications$.subscribe(n => this.notifications = n);
      },
      error: (err) => {
        console.error('available error:', err);
        this.isLoading = false;
      }
    });

    this.bloodRequestService.getMyRequests().subscribe({
      next: (res) => {
        this.myRequests = res;
        const latestRequest = res[res.length - 1] ?? null;
        if (latestRequest) {
          this.storage.set('lastRequest', JSON.stringify(latestRequest));
        } else {
          this.storage.remove('lastRequest');
        }
      },
      error: (err) => console.error('my requests error:', err)
    });

    this.donorService.getNearbyRequests().subscribe({
      next: (res) => this.nearbyRequests = res,
      error: (err) => console.error('nearby error:', err)
    });

    this.donorService.getDonationHistory().subscribe({
      next: (res) => this.donationHistory = res,
      error: (err) => console.error('history error:', err)
    });
  }

  toggleAvailability() {
    this.isAvailable = !this.isAvailable;
    this.donorService.updateAvailability(this.isAvailable).subscribe({
      next: () => console.log('availability updated:', this.isAvailable),
      error: (err) => {
        console.error('availability error:', err);
        this.isAvailable = !this.isAvailable;
      }
    });
  }

  acceptRequest(id: string) {
    this.bloodRequestService.accept(id).subscribe({
      next: () => this.loadData(),
      error: (err) => console.error('accept error:', err)
    });
  }

  cancelRequest(id: string) {
    this.bloodRequestService.cancel(id).subscribe({
      next: () => this.loadData(),
      error: (err) => console.error('cancel error:', err)
    });
  }
}