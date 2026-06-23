import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  showPassword = false;

  form = {
    email: '',
    password: ''
  };

  onSubmit() {
    const returnUrl = this.route.snapshot.queryParams['returnUrl'] as string | undefined;

    this.authService.login(this.form).subscribe({
      next: (res) => {
        this.authService.saveSession(res);

        if (returnUrl) {
          this.router.navigateByUrl(returnUrl);
          return;
        }

        if (res.role === 'Admin') this.router.navigate(['/dashboard-Admin']);
        else if (res.role === 'Hospital') this.router.navigate(['/dashboard/hospital']);
        else this.router.navigate(['/dashboard-User']);
      },
      error: (err) => {
        console.error('خطأ في تسجيل الدخول:', err);
      }
    });
  }
}