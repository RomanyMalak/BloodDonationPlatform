import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';
import {HospitalService} from "../../core/services/hospital.service";

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

async function getCoordinates(governorate: string, city: string): Promise<{ lat: number; lng: number }> {
  const query = `${city}, ${governorate}, Egypt`;
  const url = `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=1`;
  const response = await fetch(url);
  const data = await response.json();
  if (data.length > 0) {
    return { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon) };
  }
  return { lat: 0, lng: 0 };
}
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
  governorates = Object.keys(EGYPT_DATA);
  cities: string[] = [];

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
    government: '',
    city: '',
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

  onGovernorateChange() {
    this.cities = EGYPT_DATA[this.form.government] ?? [];
    this.form.city = '';
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

  async onSubmit() {
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

    const coords = await getCoordinates(this.form.government, this.form.city);

    const payload = {
      patientName: this.form.patientName,
      patientAge: this.form.patientAge ?? undefined,
      requiredBloodType: this.bloodTypeMap[this.selectedBloodType],
      urgency: this.form.urgency,
       hospitalId: this.form.hospitalId === 'other' ? undefined : this.form.hospitalId || undefined,
      customHospitalName: this.form.hospitalId === 'other' ? this.form.customHospitalName : undefined,
      government: this.form.government || undefined,
      city: this.form.city || undefined,
      latitude: coords.lat,
      longitude: coords.lng,
      medicalDocumentUrl: this.form.hospitalId === 'other' ? this.medicalFile : undefined,
      notes: this.form.notes,
      contactPhone: this.form.contactPhone,
      unitsNeeded: this.form.unitsNeeded,
      expiresAt: this.form.expiresAt || undefined
    };

    this.bloodRequestService.create(payload).subscribe({
      next: (res) => {
        console.log('تم إنشاء الطلب:', res);
        console.log(payload.latitude);
        console.log(payload.longitude);
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