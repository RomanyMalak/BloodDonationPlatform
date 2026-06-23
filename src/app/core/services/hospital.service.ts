import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { HospitalDto } from '../../shared/models/hospital.model';

@Injectable({ providedIn: 'root' })
export class HospitalService {
  private api = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getWaiting() {
    return this.http.get<HospitalDto[]>(`${this.api}/hospitals/waiting`);
  }

  getActive() {
    return this.http.get<HospitalDto[]>(`${this.api}/hospitals/active`);
  }

  getRejected() {
    return this.http.get<HospitalDto[]>(`${this.api}/hospitals/rejected`);
  }

  getById(id: string) {
    return this.http.get<HospitalDto>(`${this.api}/hospitals/${id}`);
  }

  approve(id: string) {
    return this.http.post(`${this.api}/hospitals/${id}/approve`, {});
  }

  reject(id: string, reason?: string) {
    return this.http.post(`${this.api}/hospitals/${id}/reject`, { reason });
  }

  getPendingBloodRequests() {
  return this.http.get<any[]>(`${this.api}/hospitals/requests/pendingBloodRequest`);
}

approveBloodRequest(id: string) {
  return this.http.put(`${this.api}/hospitals/requests/${id}/approveBloodRequest`, {});
}

rejectBloodRequest(id: string, reason?: string) {
  return this.http.put(`${this.api}/hospitals/requests/${id}/rejectBloodRequest`, { reason }, { responseType: 'text' as 'json' });
}
}