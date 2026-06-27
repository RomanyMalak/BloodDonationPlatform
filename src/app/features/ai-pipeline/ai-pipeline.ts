import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { BloodRequestService } from '../../core/services/blood-request.service';
import { BloodRequestDetailsDto } from '../../shared/models/blood-request.model';

@Component({
  standalone: true,
  selector: 'app-ai-pipeline',
  imports: [SidebarComponent, CommonModule],
  templateUrl: './ai-pipeline.html',
  styleUrls: ['./ai-pipeline.css'],
})
export class AiPipeline implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private bloodRequestService = inject(BloodRequestService);

  request: BloodRequestDetailsDto | null = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit() {
    const nav = this.router.getCurrentNavigation?.();
    const navRequest = nav?.extras?.state?.['request'];
    const histRequest = history.state?.request;

    if (navRequest || histRequest) {
      this.request = navRequest || histRequest;
      this.isLoading = false;
      return;
    }

    this.route.queryParams.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.loadRequest(id);
      } else {
        this.isLoading = false;
        this.errorMessage = 'لم يتم تحديد رقم الطلب.';
      }
    });
  }

 

  private loadRequest(id: string) {
    this.isLoading = true;
    this.bloodRequestService.getById(id).subscribe({
      next: res => {
        this.request = res;
        this.isLoading = false;
      },
      error: err => {
        console.error('Failed to load request', err);
        this.errorMessage = 'فشل تحميل بيانات الطلب.';
        this.isLoading = false;
      }
    });
  }

    cancelRequest() {
    if (!this.request) return;

    this.bloodRequestService.cancel(this.request.id).subscribe({
      next: () => {
        alert('تم إلغاء الطلب بنجاح.');
        this.router.navigate(['/dashboard-User']);
      },
      error: err => {
        console.error('Failed to cancel request', err);
        alert('فشل إلغاء الطلب. يرجى المحاولة مرة أخرى.');
      }
    });
  }
}
