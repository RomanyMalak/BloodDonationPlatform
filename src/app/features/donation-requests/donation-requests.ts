import { CommonModule } from '@angular/common';
import { Component, NgModule } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import {RouterLink} from '@angular/router';

interface BloodRequest {
  hospital: string;
  city: string;
  distance: string;
  time: string;
  bloodType: string;
  status: string;
  needed: string;
  progress: number;
  timeLeft: string;
}

@Component({
  selector: 'app-donation-requests',
  imports: [RouterLink ,FormsModule  , CommonModule],
  templateUrl: './donation-requests.html',
  styleUrl: './donation-requests.css',
})


export class DonationRequests {
  
   selectedBloodType = '';
  searchQuery = '';
   
    requests: BloodRequest[] = [
    {
      hospital: 'مستشفى المنيا العام',
      city: 'المنيا، شارع سعد زغلول',
      distance: '3.2 كم',
      time: '15 دقيقة',
      bloodType: '+O',
      status: 'حالة حرجة',
      needed: '5 متبرعين',
      progress: 20,
      timeLeft: 'منذ 15 دقيقة'
    },
    {
      hospital: 'مستشفى مغاغة المركزي',
      city: 'مغاغة، شارع الجمهورية',
      distance: '5.1 كم',
      time: '45 دقيقة',
      bloodType: '+A',
      status: 'عاجل',
      needed: '3 متبرعين',
      progress: 50,
      timeLeft: 'منذ 45 دقيقة'
    },
    {
      hospital: 'مستشفى دير مواس المركزي',
      city: 'دير مواس، شارع المستشفى',
      distance: '7.4 كم',
      time: '1 ساعة',
      bloodType: '+B',
      status: 'طبيعي',
      needed: '4 متبرعين',
      progress: 70,
      timeLeft: 'منذ ساعة'
    },
    {
      hospital: 'مستشفى ملوي العام',
      city: 'ملوي، شارع 26 يوليو',
      distance: '6.8 كم',
      time: '2 ساعة',
      bloodType: '-AB',
      status: 'حالة حرجة',
      needed: '2 متبرعين',
      progress: 10,
      timeLeft: 'منذ ساعتين'
    }
  ];


  filteredRequests: BloodRequest[] = [...this.requests];

  filterRequests() {
    this.filteredRequests = this.requests.filter(req => {
      const matchBlood = this.selectedBloodType
        ? req.bloodType === this.selectedBloodType
        : true;
      const matchSearch = this.searchQuery
        ? req.hospital.includes(this.searchQuery) || req.city.includes(this.searchQuery)
        : true;
      return matchBlood && matchSearch;
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'حالة حرجة': return 'badge-danger';
      case 'عاجل':       return 'badge-warning';
      case 'طبيعي':      return 'badge-success';
      default:           return 'badge-info';
    }
  }
}






