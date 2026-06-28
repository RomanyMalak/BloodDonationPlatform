import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { BloodRequestSummaryDto } from '../../shared/models/blood-request.model';

@Component({
  selector: 'app-donation-requests',
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './donation-requests.html',
  styleUrl: './donation-requests.css',
})
export class DonationRequests implements OnInit {

  private bloodRequestService = inject(BloodRequestService);

  selectedBloodType = '';
  searchQuery = '';
  isLoading = false;

  requests: BloodRequestSummaryDto[] = [];
  filteredRequests: BloodRequestSummaryDto[] = [];

  ngOnInit() {
    this.loadRequests();
  }

  loadRequests() {
    this.isLoading = true;
    this.bloodRequestService.getAvailable().subscribe({
      next: (res) => {
        this.requests = res;
        this.filteredRequests = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('requests error:', err);
        this.isLoading = false;
      }
    });
  }

  filterRequests() {
    this.filteredRequests = this.requests.filter(req => {
      const matchBlood = this.selectedBloodType
        ? req.requiredBloodType === this.selectedBloodType
        : true;
      const matchSearch = this.searchQuery
        ? req.patientName?.includes(this.searchQuery) || req.hospitalName?.includes(this.searchQuery)
        : true;
      return matchBlood && matchSearch;
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Critical': return 'badge-danger';
      case 'Urgent':   return 'badge-warning';
      case 'Normal':   return 'badge-success';
      default:         return 'badge-info';
    }
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'Critical': 'حالة حرجة',
      'Urgent': 'عاجل',
      'Normal': 'طبيعي',
      'Matching': 'جاري المطابقة',
      'Completed': 'مكتمل',
      'Cancelled': 'ملغي'
    };
    return labels[status] || status;
  }
}