import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { BloodRequestService } from '../../core/services/blood-request.service';

@Component({
  selector: 'app-request-detail',
  standalone: true,
  imports: [RouterLink, SidebarComponent, CommonModule],
  templateUrl: './request-detail.html',
  styleUrl: './request-detail.css'
})
export class RequestDetailComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private bloodRequestService = inject(BloodRequestService);

  request: any = {};

  ngOnInit() {
    // Prefer router navigation extras if available (fast), otherwise fetch by id from API
    const nav = this.router.getCurrentNavigation?.();
    const navRequest = nav && nav.extras && nav.extras.state && nav.extras.state['request'];
    const histRequest = (history && history.state && history.state['request']);

    console.log(this.request);
    if (navRequest || histRequest) {
      this.request = navRequest || histRequest;
      return;
    }

    const id = this.route.snapshot.queryParams['id'];
    if (id) {
      this.bloodRequestService.getById(id).subscribe({
        next: (res) => this.request = res,
        error: (err) => {
          console.error('Failed to load request by id', err);
          this.request = {};
        }
      });
    }
  }

  private bloodTypeMapReverse: Record<number, string> = {
    0: 'A+', 1: 'A-', 2: 'B+', 3: 'B-', 4: 'AB+', 5: 'AB-', 6: 'O+', 7: 'O-'
  };

  getBloodTypeSymbol(value: number | string | undefined) {
    if (value === undefined || value === null) return undefined;

    const lookup: Record<string, string> = {
      'A+': 'A+',
      'A-': 'A-',
      'B+': 'B+',
      'B-': 'B-',
      'AB+': 'AB+',
      'AB-': 'AB-',
      'O+': 'O+',
      'O-': 'O-',
      'APositive': 'A+',
      'ANegative': 'A-',
      'BPositive': 'B+',
      'BNegative': 'B-',
      'ABPositive': 'AB+',
      'ABNegative': 'AB-',
      'OPositive': 'O+',
      'ONegative': 'O-'
    };

    if (typeof value === 'number') {
      return this.bloodTypeMapReverse[value] || String(value);
    }

    const normalized = value.trim();
    return lookup[normalized] || lookup[normalized.toUpperCase()] || normalized || undefined;
  }
}