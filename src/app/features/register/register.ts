import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { BLOOD_TYPES } from '../../shared/models/blood-type.enum';
import { AuthService } from '../../core/services/auth.service';
import { RegisterHospitalRequest } from '../../shared/models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  licenseFile: File | null = null;

  private authService = inject(AuthService);
  private router = inject(Router);
  


  bloodTypes = BLOOD_TYPES;
  step = 1;
  selectedType = '';
  showPassword = false;
  showConfirmPassword = false;
  selectedFileName = '';
  showSuccessMessage = false;


form = {
  name: '',
  email: '',
  phone: '',
  bloodType: null as number | null,  
  password: '',
  confirmPassword: '',
  age: null as number | null,        
  terms: false,
  hospitalName: '',
  licenseNumber: '',
  landline: '',
  city: '',
  addressDetail: '',
  government: ''
};
  selectType(type: string) {
    this.selectedType = type;
  }

  nextStep() {
    if (this.selectedType) this.step = 2;
  }

  getTypeLabel(): string {
    const labels: Record<string, string> = {
      user: ' مريض / متبرع',
      hospital: 'مستشفى / مركز صحي'
    };
    return labels[this.selectedType] || '';
  }


onFileSelect(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files?.length) {
    this.licenseFile = input.files[0];
    this.selectedFileName = input.files[0].name;
  }
}


  passwordsMatch(): boolean {
    return this.form.password === this.form.confirmPassword && this.form.password !== '';
  }

  // ===== VALIDATION METHODS =====
  validateName(name: string): boolean {
    if (!name) return false;
    // Must contain at least 3 words (separated by spaces)
    const words = name.trim().split(/\s+/);
    const nameRegex = /^[a-zA-Zأ-ي\s]{3,}$/;
    return nameRegex.test(name) && words.length >= 3;
  }

  validateEmail(email: string): boolean {
    if (!email) return false;
    // Standard email format validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  validatePhone(phone: string): boolean {
    if (!phone) return false;
    // Egyptian phone: 01X XXXXXXXX (11 digits total, starts with 01)
    // X can be 0, 1, 2, or 5
    const phoneRegex = /^01[0-2,5]\d{8}$/;
    return phoneRegex.test(phone.replace(/\s/g, ''));
  }

  validateLicenseNumber(license: string): boolean {
    if (!license) return false;
    // 6 to 10 digits only
    const licenseRegex = /^\d{6,10}$/;
    return licenseRegex.test(license);
  }

  validateLandline(landline: string): boolean {
    if (!landline) return false;
    // Egyptian landline: 0XX XXXXXXX (10 digits, starts with 0)
    const landlineRegex = /^0[1-9]\d{8}$/;
    return landlineRegex.test(landline.replace(/\s/g, ''));
  }

  isNameValid(): boolean {
    return this.form.name === '' || this.validateName(this.form.name);
  }

  isEmailValid(): boolean {
    return this.form.email === '' || this.validateEmail(this.form.email);
  }

  isPhoneValid(): boolean {
    return this.form.phone === '' || this.validatePhone(this.form.phone);
  }

  isLicenseValid(): boolean {
    return this.form.licenseNumber === '' || this.validateLicenseNumber(this.form.licenseNumber);
  }

  isLandlineValid(): boolean {
    return this.form.landline === '' || this.validateLandline(this.form.landline);
  }

 

onSubmit() {
  
  if (!this.passwordsMatch()) return;

    // register user

  if (this.selectedType === 'user') {
   const payload = {
  fullName: this.form.name,
  email: this.form.email,
  password: this.form.password,
  phone: this.form.phone || undefined,
  age: this.form.age ?? undefined,
 bloodType: this.form.bloodType ? Number(this.form.bloodType) : undefined,
  role: 0,
  latitude: 0,
  longitude: 0
};
    this.authService.registerUser(payload).subscribe({
      next: (res) => {
        console.log('تم التسجيل:', res);
        console.log(res.token)
         this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('خطأ:', err);
      }
    });
  }

  // register Hospital
    if (this.selectedType === 'hospital') {
  const payload: RegisterHospitalRequest = {
    email: this.form.email,
    password: this.form.password,
    hospitalName: this.form.hospitalName,
    government: this.form.government,
    city: this.form.city,
    addressDetail: this.form.addressDetail,
    latitude: 0,
    longitude: 0,
    hotline: this.form.landline,
    licenseDocumentPath: this.licenseFile!,
    
  };

  this.authService.registerHospital(payload).subscribe({
    next: (res) => {
      console.log('تم تسجيل المستشفى:', res);
      this.showSuccessMessage = true;
    },
    error: (err) => {
      console.error('Registration error:', err);
    }
  });
}
}}