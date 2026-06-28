import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateAdminRequest, DashboardStats } from '../../../app/shared/models/dashboard.model';
import { AdminDto } from '../../../app/shared/models/dashboard.model'; 
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private api = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  createAdmin(request: CreateAdminRequest): Observable<any> {
    return this.http.post(`${this.api}/create-admin`, request, { responseType: 'text' });
  }

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.api}/stats`);
  }

  getAiLogs(): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/ai-logs`);
  }

  getAdmins(): Observable<AdminDto[]> {
    return this.http.get<AdminDto[]>(`${this.api}/admins`);
  }
}
 