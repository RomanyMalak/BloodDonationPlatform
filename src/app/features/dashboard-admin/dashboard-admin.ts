import { HospitalDto } from './../../shared/models/hospital.model';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { HospitalService } from '../../core/services/hospital.service';


@Component({
  selector: 'app-dashboard-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent ],
  templateUrl: './dashboard-admin.html',
  styleUrl: './dashboard-admin.css'
})
export class DashboardAdmin implements OnInit {

  private hospitalService = inject(HospitalService);

  activeTab = 'hospitals';
  newAdmin = { name: '', email: '', role: '' };
  showRejectModal = false;
rejectReason = '';
selectedHospitalId = '';

pendingHospitals: HospitalDto[] = [];
approvedHospitals: HospitalDto[] = [];
rejectedHospitals: HospitalDto[] = [];
  isLoading = false;

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.isLoading = true;

    this.hospitalService.getWaiting().subscribe({
      next: (res) => {
        console.log('waiting:', res);
        this.pendingHospitals = res;
      },
      error: (err) => console.error('waiting error:', err)
    });

    this.hospitalService.getActive().subscribe({
      next: (res) => {
        console.log('active:', res);
        this.approvedHospitals = res;
        this.isLoading = false;
      },
      error: (err) => { console.error('active error:', err); this.isLoading = false; }
    });

    this.hospitalService.getRejected().subscribe({
      next: (res) => {
        console.log('rejected:', res);
        this.rejectedHospitals = res;
      },
      error: (err) => console.error('rejected error:', err)
    });
  }

  approveHospital(id: string) {
    this.hospitalService.approve(id).subscribe({
      next: () => this.loadAll(),
      error: (err) => console.error('approve error:', err)
    });
  }

  rejectHospital(id: string, reason?: string) {
    this.hospitalService.reject(id, reason).subscribe({
      next: () => this.loadAll(),
      error: (err) => console.error('reject error:', err)
    });
  }


  revokeHospital(id: string) {
  this.hospitalService.reject(id).subscribe({
    next: () => this.loadAll(),
    error: (err) => console.error('revoke error:', err)
  });
}   


  addAdmin() { }
  removeAdmin(id: number) { }
  getRoleLabel(role: string): string {
    const labels: Record<string, string> = {
      super_admin: 'Super Admin',
      moderator: 'Moderator',
      support: 'Support'
    };
    return labels[role] || role;
  }

openRejectModal(id: string) {
  this.selectedHospitalId = id;
  this.showRejectModal = true;
}

confirmReject() {
  this.hospitalService.reject(this.selectedHospitalId, this.rejectReason).subscribe({
    next: () => {
      this.loadAll();
      this.showRejectModal = false;
      this.rejectReason = '';
      this.selectedHospitalId = '';
    },
    error: (err) => console.error('reject error:', err)
  });
}
closeRejectModal() {
  this.showRejectModal = false;
  this.rejectReason = '';
  this.selectedHospitalId = '';
}
}