import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { BLOOD_TYPES } from '../../shared/models/blood-type.enum';
import { AuthService } from '../../core/services/auth.service';
import { RegisterHospitalRequest } from '../../shared/models/auth.model';

// ===== Egypt Governorates & Cities Data =====
const EGYPT_DATA: Record<string, string[]> = {
  'القاهرة': ['مدينة نصر', 'المعادي', 'الزيتون', 'شبرا', 'عين شمس', 'مصر الجديدة', 'المطرية', 'التجمع الأول', 'التجمع الخامس', 'حلوان', 'المقطم', 'الأميرية', 'السلام', 'دار السلام'],
  'الجيزة': ['الدقي', 'المهندسين', 'العجوزة', 'إمبابة', 'بولاق الدكرور', 'الهرم', 'فيصل', 'الشيخ زايد', '6 أكتوبر', 'أوسيم', 'كرداسة', 'البدرشين'],
  'الإسكندرية': ['المنتزه', 'العجمي', 'الجمرك', 'باب شرق', 'محرم بك', 'سيدي بشر', 'ستانلي', 'المعمورة', 'برج العرب', 'الدخيلة'],
  'الدقهلية': ['المنصورة', 'طلخا', 'ميت غمر', 'دكرنس', 'أجا', 'المنزلة', 'بلقاس', 'السنبلاوين', 'شربين', 'تمي الأمديد'],
  'الشرقية': ['الزقازيق', 'بلبيس', 'منيا القمح', 'أبو كبير', 'فاقوس', 'الإسماعيلية', 'ههيا', 'ديرب نجم', '10 رمضان'],
  'القليوبية': ['بنها', 'شبرا الخيمة', 'قليوب', 'القناطر الخيرية', 'طوخ', 'الخانكة', 'كفر شكر'],
  'الغربية': ['طنطا', 'المحلة الكبرى', 'كفر الزيات', 'زفتى', 'السنطة', 'بسيون', 'سمنود'],
  'المنوفية': ['شبين الكوم', 'مينوف', 'أشمون', 'الشهداء', 'قويسنا', 'بركة السبع', 'تلا', 'السادات'],
  'البحيرة': ['دمنهور', 'كفر الدوار', 'رشيد', 'إدكو', 'أبو المطامير', 'الدلنجات', 'شبراخيت', 'وادي النطرون'],
  'الإسماعيلية': ['الإسماعيلية', 'فايد', 'القنطرة', 'أبو صوير', 'التل الكبير'],
  'السويس': ['السويس', 'عتاقة', 'الجناين', 'فيصل'],
  'بورسعيد': ['بورسعيد', 'بورفؤاد', 'الزهور', 'المناخ', 'الشرق'],
  'دمياط': ['دمياط', 'رأس البر', 'فارسكور', 'الزرقا', 'كفر سعد'],
  'كفر الشيخ': ['كفر الشيخ', 'دسوق', 'فوه', 'قلين', 'مطوبس', 'بيلا', 'الحامول'],
  'أسيوط': ['أسيوط', 'ديروط', 'منفلوط', 'القوصية', 'أبنوب', 'إبراهيم الكبير', 'البداري', 'ساحل سليم'],
  'سوهاج': ['سوهاج', 'أخميم', 'طهطا', 'المراغة', 'جرجا', 'دشنا', 'البلينا', 'المنشأة'],
  'قنا': ['قنا', 'نجع حمادي', 'قوص', 'دشنا', 'أبو تشت', 'الوقف'],
  'الأقصر': ['الأقصر', 'إسنا', 'أرمنت', 'الطود', 'القرنة'],
  'أسوان': ['أسوان', 'كوم أمبو', 'إدفو', 'نصر النوبة', 'دراو'],
  'المنيا': ['المنيا', 'ملوي', 'مغاغة', 'بني مزار', 'أبو قرقاص', 'سمالوط', 'العدوة', 'مطاي'],
  'بني سويف': ['بني سويف', 'الفيوم', 'ناصر', 'إهناسيا', 'ببا', 'سمسطا', 'الواسطى'],
  'الفيوم': ['الفيوم', 'سنورس', 'إطسا', 'يوسف الصديق', 'طامية'],
  'شمال سيناء': ['العريش', 'رفح', 'الشيخ زويد', 'بئر العبد', 'نخل'],
  'جنوب سيناء': ['الطور', 'شرم الشيخ', 'دهب', 'نويبع', 'طابا', 'أبو رديس'],
  'البحر الأحمر': ['الغردقة', 'سفاجا', 'القصير', 'مرسى علم', 'رأس غارب'],
  'مطروح': ['مرسى مطروح', 'سيوة', 'الضبعة', 'السلوم', 'النجيلة'],
  'الوادي الجديد': ['الخارجة', 'الداخلة', 'الفرافرة', 'بريس'],
};

// ===== Geocoding via Nominatim =====
async function getCoordinates(governorate: string, city: string): Promise<{ lat: number; lng: number }> {
  const query = `${city}, ${governorate}, Egypt`;
  const url = `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=1`;
  const response = await fetch(url);
  const data = await response.json();
  if (data.length > 0) {
    return { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon) };
  }
  // fallback لو مش لاقي نتيجة
  return { lat: 0, lng: 0 };
}

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

  // ===== Governorates & Cities =====
  governorates = Object.keys(EGYPT_DATA);
  cities: string[] = [];

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

  // ===== لما اليوزر يغير المحافظة =====
  onGovernorateChange() {
    this.cities = EGYPT_DATA[this.form.government] ?? [];
    this.form.city = '';
  }

  selectType(type: string) {
    this.selectedType = type;
  }

  nextStep() {
    if (this.selectedType) this.step = 2;
  }

  getTypeLabel(): string {
    const labels: Record<string, string> = {
      user: 'مريض / متبرع',
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
    const words = name.trim().split(/\s+/);
    const nameRegex = /^[a-zA-Zأ-ي\s]{3,}$/;
    return nameRegex.test(name) && words.length >= 3;
  }

  validateEmail(email: string): boolean {
    if (!email) return false;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  validatePhone(phone: string): boolean {
    if (!phone) return false;
    const phoneRegex = /^01[0-2,5]\d{8}$/;
    return phoneRegex.test(phone.replace(/\s/g, ''));
  }

  validateLicenseNumber(license: string): boolean {
    if (!license) return false;
    const licenseRegex = /^\d{6,10}$/;
    return licenseRegex.test(license);
  }

  validateLandline(landline: string): boolean {
    if (!landline) return false;
    const landlineRegex = /^0[1-9]\d{8}$/;
    return landlineRegex.test(landline.replace(/\s/g, ''));
  }

  isNameValid(): boolean { return this.form.name === '' || this.validateName(this.form.name); }
  isEmailValid(): boolean { return this.form.email === '' || this.validateEmail(this.form.email); }
  isPhoneValid(): boolean { return this.form.phone === '' || this.validatePhone(this.form.phone); }
  isLicenseValid(): boolean { return this.form.licenseNumber === '' || this.validateLicenseNumber(this.form.licenseNumber); }
  isLandlineValid(): boolean { return this.form.landline === '' || this.validateLandline(this.form.landline); }

  // ===== SUBMIT =====
  async onSubmit() {
    if (!this.passwordsMatch()) return;

    // ===== Register User =====
    if (this.selectedType === 'user') {
      const coords = await getCoordinates(this.form.government, this.form.city);

      const payload = {
        fullName: this.form.name,
        email: this.form.email,
        password: this.form.password,
        phone: this.form.phone || undefined,
        age: this.form.age ?? undefined,
        bloodType: this.form.bloodType ? Number(this.form.bloodType) : undefined,
        role: 0,
        latitude: coords.lat,
        longitude: coords.lng
      };

      this.authService.registerUser(payload).subscribe({
        next: (res) => {
          console.log('تم التسجيل:', res);
          console.log(payload.longitude);
          console.log(payload.latitude);
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('خطأ:', err);
        }
      });
    }

    // ===== Register Hospital =====
    if (this.selectedType === 'hospital') {
      const coords = await getCoordinates(this.form.government, this.form.city);

      const payload: RegisterHospitalRequest = {
        email: this.form.email,
        password: this.form.password,
        hospitalName: this.form.hospitalName,
        government: this.form.government,
        city: this.form.city,
        addressDetail: this.form.addressDetail,
        latitude: coords.lat,
        longitude: coords.lng,
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
  }
}