import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';
import {HospitalService} from "../../core/services/hospital.service";
@Component({
  selector: 'app-request-form',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule ],
  templateUrl: './request-form.html',
  styleUrls: ['./request-form.css']
})
export class RequestFormComponent implements OnInit {

  private bloodRequestService = inject(BloodRequestService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private platformId = inject(PLATFORM_ID);
  private hospitalService = inject(HospitalService);

  isLoading = false;
  isLoggedIn = false;
  requireLogin = false;
  medicalFile: File | null = null;
  selectedFileName = '';
  selectedUrgency = 'normal';
  selectedBloodType = '';
  activeHospitals: any[] = [];

  urgencyMap: Record<string, number> = { normal: 0, urgent: 1, critical: 2 };
  bloodTypeMap: Record<string, number> = {
    '+A': 0, '-A': 1, '+B': 2, '-B': 3,
    '+AB': 4, '-AB': 5, '+O': 6, '-O': 7
  };

  form = {
    patientName: '',
    patientAge: null as number | null,
    bloodType: null as number | null,
    urgency: 0,
    customHospitalName: '',
    hospitalId: '',
    contactPhone: '',
    notes: '',
    unitsNeeded: 1,
    expiresAt: ''
  };

ngOnInit() {
  this.route.queryParams.subscribe(params => {
    this.requireLogin = params['requireLogin'] === 'true';
  });
  if (isPlatformBrowser(this.platformId)) {
    const token = localStorage.getItem('token');
    if (token) {
      this.form.patientName = localStorage.getItem('fullName') || '';
      this.form.contactPhone = localStorage.getItem('phone') || '';
    }
  }
  this.loadHospitals();
}

  loadHospitals() {
  this.hospitalService.getActive().subscribe({
    next: (res) => this.activeHospitals = res,
    error: (err) => console.error('hospitals error:', err)
  });
}


onHospitalSelect(event: Event) {
  const select = event.target as HTMLSelectElement;
  const selected = this.activeHospitals.find(h => h.id === select.value);
  if (selected) {
    this.form.hospitalId = selected.id;
    this.form.customHospitalName = '';
  }
} 
  setUrgency(val: string) {
    this.selectedUrgency = val;
    this.form.urgency = this.urgencyMap[val];
  }

  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.medicalFile = input.files[0];
      this.selectedFileName = input.files[0].name;
    }
  }

  onSubmit() {
    if (!this.medicalFile) return;
    this.isLoading = true;

    const payload = {
      patientName: this.form.patientName,
      patientAge: this.form.patientAge ?? undefined,
      requiredBloodType: this.bloodTypeMap[this.selectedBloodType],
      urgency: this.form.urgency,
       hospitalId: this.form.hospitalId || undefined,
      customHospitalName: this.form.customHospitalName,
      latitude: 0,
      longitude: 0,
      medicalDocumentUrl: this.medicalFile,
      notes: this.form.notes,
      contactPhone: this.form.contactPhone,
      unitsNeeded: this.form.unitsNeeded,
      expiresAt: this.form.expiresAt || undefined
    };

    if (!this.isLoggedIn) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/request-form' } });
      this.isLoading = false;
      return;
    }

    this.bloodRequestService.create(payload).subscribe({
      next: (res) => {
        console.log('تم إنشاء الطلب:', res);
        this.isLoading = false;
        // navigate to request-detail by id so the detail page can fetch data from API
        const id = res && (res.id || res.requestId || res.data && res.data.id);
        if (id) {
          this.router.navigate(['/request-detail'], { queryParams: { id } });
        } else {
          // fallback to navigation state when id not available
          this.router.navigate(['/request-detail'], { state: { request: res } });
        }
      },
      error: (err) => {
        console.error('خطأ:', err);
        this.isLoading = false;
      }
    });
  }
}