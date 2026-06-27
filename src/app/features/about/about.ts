import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-about',
  imports: [CommonModule, FormsModule],
  templateUrl: './about.html',
  styleUrl: './about.css',
})
export class About {
  showPartnershipForm = false;
  partnershipForm = {
    name: '',
    email: '',
    phone: ''
  };

  openPartnershipForm() {
    this.showPartnershipForm = true;
  }

  closePartnershipForm() {
    this.showPartnershipForm = false;
    this.partnershipForm = { name: '', email: '', phone: '' };
  }

  submitPartnershipForm() {
    if (!this.partnershipForm.name || !this.partnershipForm.email || !this.partnershipForm.phone) {
      return;
    }

    alert('تم إرسال طلب الشراكة بنجاح.');
    this.closePartnershipForm();
  }
}
