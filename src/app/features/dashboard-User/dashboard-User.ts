import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { DonorService } from '../../core/services/donor.service';
import { BloodRequestSummaryDto, DonorNearbyRequestDto } from '../../shared/models/blood-request.model';
import { StorageService } from '../../core/services/storage.service';

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
  fullName = '';

  isAvailable = true;
  isLoading = false;

  availableRequests: BloodRequestSummaryDto[] = [];
  myRequests: BloodRequestSummaryDto[] = [];
  nearbyRequests: DonorNearbyRequestDto[] = [];
  donationHistory: any[] = [];

  ngOnInit() {
    this.fullName = this.storage.get('fullName') || 'مستخدم';
    this.loadData();
  }

  loadData() {
    const token = this.storage.get('token');
    if (!token) return;

    this.isLoading = true;

    this.bloodRequestService.getAvailable().subscribe({
      next: (res) => {
        this.availableRequests = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('available error:', err);
        this.isLoading = false;
      }
    });

    this.bloodRequestService.getMyRequests().subscribe({
  next: (res) => {
    console.log('my requests:', res);
    this.myRequests = res;
  },
  error: (err) => console.error('my requests error:', err)
}); 
    this.donorService.getNearbyRequests().subscribe({
      next: (res) => {
        console.log('nearby:', res);
        this.nearbyRequests = res;
      },
      error: (err) => console.error('nearby error:', err)
    });

    this.donorService.getDonationHistory().subscribe({
      next: (res) => {
        console.log('history:', res);
        this.donationHistory = res;
      },
      error: (err) => console.error('history error:', err)
    });
  }

  toggleAvailability() {
    this.isAvailable = !this.isAvailable;
    this.donorService.updateAvailability(this.isAvailable).subscribe({
      next: () => console.log('availability updated:', this.isAvailable),
      error: (err) => {
        console.error('availability error:', err);
        this.isAvailable = !this.isAvailable; // rollback لو فشل
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