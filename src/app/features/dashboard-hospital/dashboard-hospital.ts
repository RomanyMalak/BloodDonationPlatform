import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HospitalService } from '../../core/services/hospital.service';
import { StorageService } from '../../core/services/storage.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard-hospital',
  standalone: true,
  imports: [CommonModule, RouterLink , FormsModule],
  templateUrl: './dashboard-hospital.html',
  styleUrl: './dashboard-hospital.css'
})
export class DashboardHospital implements OnInit {

  private hospitalService = inject(HospitalService);
  private storage = inject(StorageService);

  hospitalName = '';
  pendingRequests: any[] = [];
  isLoading = false;

  showRejectModal = false;
  rejectReason = '';
  selectedRequestId = '';

  ngOnInit() {
    this.hospitalName = this.storage.get('fullName') || 'المستشفى';
    this.loadData();
  }

  loadData() {
    const token = this.storage.get('token');
    if (!token) return;

    this.isLoading = true;
    this.hospitalService.getPendingBloodRequests().subscribe({
      next: (res) => {
        console.log('pending requests:', res);
        this.pendingRequests = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('pending error:', err);
        this.isLoading = false;
      }
    });
  }

  approveRequest(id: string) {
    this.hospitalService.approveBloodRequest(id).subscribe({
      next: () => this.loadData(),
      error: (err) => console.error('approve error:', err)
    });
  }

  openRejectModal(id: string) {
    this.selectedRequestId = id;
    this.showRejectModal = true;
  }

  confirmReject() {
    this.hospitalService.rejectBloodRequest(this.selectedRequestId, this.rejectReason).subscribe({
      next: () => {
        this.loadData();
        this.showRejectModal = false;
        this.rejectReason = '';
        this.selectedRequestId = '';
      },
      error: (err) => console.error('reject error:', err)
    });
  }

  closeRejectModal() {
    this.showRejectModal = false;
    this.rejectReason = '';
    this.selectedRequestId = '';
  }
}