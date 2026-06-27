import { HospitalDto } from './../../shared/models/hospital.model';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HospitalService } from '../../core/services/hospital.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats, CreateAdminRequest } from '../../shared/models/dashboard.model';

@Component({
  selector: 'app-dashboard-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard-admin.html',
  styleUrl: './dashboard-admin.css'
})
export class DashboardAdmin implements OnInit {

  private hospitalService = inject(HospitalService);
  private dashboardService = inject(DashboardService);

  activeTab = 'hospitals';

  newAdmin: CreateAdminRequest = { fullName: '', email: '', password: '', phone: '' };
  adminMessage: string | null = null;
  adminErrorMessage: string | null = null;

  showRejectModal = false;
  rejectReason = '';
  selectedHospitalId = '';

  pendingHospitals: HospitalDto[] = [];
  approvedHospitals: HospitalDto[] = [];
  rejectedHospitals: HospitalDto[] = [];

  stats: DashboardStats | null = null;
  aiLogs: any[] = [];

  isLoading = false;

  ngOnInit() {
    this.loadAll();
    this.loadStats();
    this.loadAiLogs();
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

  loadStats() {
    this.dashboardService.getStats().subscribe({
      next: (res) => this.stats = res,
      error: (err) => console.error('stats error:', err)
    });
  }

  loadAiLogs() {
    this.dashboardService.getAiLogs().subscribe({
      next: (res) => this.aiLogs = res,
      error: (err) => console.error('ai logs error:', err)
    });
  }

  addAdmin() {
    this.adminMessage = null;
    this.adminErrorMessage = null;

    this.dashboardService.createAdmin(this.newAdmin).subscribe({
      next: (res) => {
        console.log('admin created:', res);
        this.adminMessage = res;
        this.adminErrorMessage = null;
        this.newAdmin = { fullName: '', email: '', password: '', phone: '' };
      },
      error: (err) => {
        const message = err?.error || err?.message || 'حدث خطأ أثناء إنشاء الأدمن';
        this.adminErrorMessage = typeof message === 'string' ? message : 'حدث خطأ أثناء إنشاء الأدمن';
        console.error('create admin error:', err);
      }
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