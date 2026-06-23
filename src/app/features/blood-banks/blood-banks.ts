import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface BloodBank {
  name: string;
  city: string;
  type: string;
  region: string;
  mapUrl: string;
  phone:string;
}

@Component({
  selector: 'app-blood-banks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './blood-banks.html',
  styleUrl: './blood-banks.css'
})
export class BloodBanks {

  searchQuery = '';
  selectedCity = '';

banks: BloodBank[] = [
  { name: 'المركز القومي — العجوزة',    city: 'الجيزة',      type: 'قومي',      region: 'القاهرة',    phone: '0227940000', mapUrl: 'https://maps.google.com/?q=المركز+القومي+للدم+العجوزة+الجيزة' },
  { name: 'الإقليمي — العباسية',        city: 'القاهرة',     type: 'إقليمي',    region: 'القاهرة',    phone: '0224820000', mapUrl: 'https://maps.google.com/?q=بنك+دم+العباسية+القاهرة' },
  { name: 'الإقليمي — الإسكندرية',      city: 'الإسكندرية',  type: 'إقليمي',    region: 'الإسكندرية', phone: '0345670000', mapUrl: 'https://maps.google.com/?q=بنك+دم+الإسكندرية' },
  { name: 'الإقليمي — المنصورة',        city: 'المنصورة',    type: 'إقليمي',    region: 'الدلتا',     phone: '0502340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+المنصورة' },
  { name: 'الإقليمي — الإسماعيلية',     city: 'الإسماعيلية', type: 'إقليمي',    region: 'الدلتا',     phone: '0643210000', mapUrl: 'https://maps.google.com/?q=بنك+دم+الإسماعيلية' },
  { name: 'الإقليمي — طنطا',            city: 'طنطا',        type: 'إقليمي',    region: 'الدلتا',     phone: '0403330000', mapUrl: 'https://maps.google.com/?q=بنك+دم+طنطا' },
  { name: 'الإقليمي — المنيا',          city: 'المنيا',      type: 'إقليمي',    region: 'الصعيد',     phone: '0862340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+المنيا' },
  { name: 'الإقليمي — أسوان',           city: 'أسوان',       type: 'إقليمي',    region: 'الصعيد',     phone: '0972340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+أسوان' },
  { name: 'الإقليمي — سوهاج',           city: 'سوهاج',       type: 'إقليمي',    region: 'الصعيد',     phone: '0932340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+سوهاج' },
  { name: 'الإقليمي — دمنهور',          city: 'دمنهور',      type: 'إقليمي',    region: 'الدلتا',     phone: '0452340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+دمنهور' },
  { name: 'الإقليمي — الزقازيق',        city: 'الشرقية',     type: 'إقليمي',    region: 'الدلتا',     phone: '0552340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+الزقازيق' },
  { name: 'الإقليمي — بنها',            city: 'القليوبية',   type: 'إقليمي',    region: 'القاهرة',    phone: '0132340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+بنها+القليوبية' },
  { name: 'الإقليمي — كفر الشيخ',       city: 'كفر الشيخ',   type: 'إقليمي',    region: 'الدلتا',     phone: '0472340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+كفر+الشيخ' },
  { name: 'الإقليمي — بني سويف',        city: 'بني سويف',    type: 'إقليمي',    region: 'الصعيد',     phone: '0822340000', mapUrl: '  ' },
  { name: 'شبين الكوم — المنوفية',      city: 'المنوفية',    type: 'إقليمي',    region: 'الدلتا',     phone: '0482340000', mapUrl: 'https://maps.google.com/?q=بنك+دم+شبين+الكوم+المنوفية' },
  { name: 'هلال أحمر — القاهرة',        city: 'القاهرة',     type: 'هلال أحمر', region: 'القاهرة',    phone: '0227920000', mapUrl: 'https://maps.google.com/?q=الهلال+الأحمر+المصري+القاهرة' },
  { name: 'هلال أحمر — الإسكندرية',     city: 'الإسكندرية',  type: 'هلال أحمر', region: 'الإسكندرية', phone: '0345680000', mapUrl: 'https://maps.google.com/?q=الهلال+الأحمر+المصري+الإسكندرية' },
  { name: 'هلال أحمر — طنطا',           city: 'طنطا',        type: 'هلال أحمر', region: 'الدلتا',     phone: '0403340000', mapUrl: 'https://maps.google.com/?q=الهلال+الأحمر+المصري+طنطا' },
  { name: 'دار السلام — القاهرة',       city: 'القاهرة',     type: 'عام',       region: 'القاهرة',    phone: '0221230000', mapUrl: 'https://maps.google.com/?q=مستشفى+دار+السلام+القاهرة' },
];
  filteredBanks: BloodBank[] = [...this.banks];

  selectCity(city: string) {
    this.selectedCity = city;
    this.filterBanks();
  }

  filterBanks() {
    this.filteredBanks = this.banks.filter(bank => {
      const matchCity = this.selectedCity
        ? bank.region === this.selectedCity || bank.city === this.selectedCity
        : true;
      const matchSearch = this.searchQuery
        ? bank.name.includes(this.searchQuery) || bank.city.includes(this.searchQuery)
        : true;
      return matchCity && matchSearch;
    });
  }
}