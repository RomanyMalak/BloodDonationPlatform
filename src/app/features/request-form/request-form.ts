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

  errorMessage: string = '';
  urgencyMap: Record<string, number> = { normal: 0, urgent: 1, critical: 2 };
  bloodTypeMap: Record<string, number> = {
    'O+': 0,
    'O-': 1,
    'A+': 2,
    'A-': 3,
    'B+': 4,
    'B-': 5,
    'AB+': 6,
    'AB-': 7
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
    this.isLoggedIn = !!token;

    if (!this.isLoggedIn) {
      this.requireLogin = true;
    }

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
  const selectedValue = select.value;
  this.form.hospitalId = selectedValue;

  if (selectedValue === 'other') {
    this.form.customHospitalName = '';
    return;
  }

  const selected = this.activeHospitals.find(h => h.id === selectedValue);
  if (selected) {
    this.form.hospitalId = selected.id;
    this.form.customHospitalName = '';
  }
}
  setUrgency(val: string) {
    this.selectedUrgency = val;
    this.form.urgency = this.urgencyMap[val];
  }

  validateContactPhone(): boolean {
    // Phone validation: Egyptian phone number format (11 digits starting with 01)
    const phoneRegex = /^01[0-9]{9}$/;
    return phoneRegex.test(this.form.contactPhone);
  }

  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.medicalFile = input.files[0];
      this.selectedFileName = input.files[0].name;
    }
  }

  onSubmit() {
    if (this.form.hospitalId === 'other' && !this.medicalFile) return;

    if (!this.isLoggedIn) {
      this.requireLogin = true;
      return;
    }

    // Validate contactPhone
    if (!this.form.contactPhone || !this.validateContactPhone()) {
      this.errorMessage = 'رقم الهاتف غير صحيح. الرجاء إدخال رقم صحيح (مثال: 01012345678)';
      setTimeout(() => {
        this.errorMessage = '';
      }, 5000);
      return;
    }

    this.isLoading = true;

    const payload = {
      patientName: this.form.patientName,
      patientAge: this.form.patientAge ?? undefined,
      requiredBloodType: this.bloodTypeMap[this.selectedBloodType],
      urgency: this.form.urgency,
       hospitalId: this.form.hospitalId === 'other' ? undefined : this.form.hospitalId || undefined,
      customHospitalName: this.form.hospitalId === 'other' ? this.form.customHospitalName : undefined,
      latitude: 0,
      longitude: 0,
      medicalDocumentUrl: this.form.hospitalId === 'other' ? this.medicalFile : undefined,
      notes: this.form.notes,
      contactPhone: this.form.contactPhone,
      unitsNeeded: this.form.unitsNeeded,
      expiresAt: this.form.expiresAt || undefined
    };

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
        // Display backend error message
        this.errorMessage = err.error?.message || err.message || 'حدث خطأ أثناء إنشاء الطلب';
        // Clear error after 5 seconds
        setTimeout(() => {
          this.errorMessage = '';
        }, 5000);
      }
    });
  }
}