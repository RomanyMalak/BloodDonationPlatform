import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { DonorNearbyRequestDto } from '../../shared/models/blood-request.model';

@Injectable({ providedIn: 'root' })
export class DonorService {
  private api = `${environment.apiUrl}/Donors`;

  constructor(private http: HttpClient) {}

  getNearbyRequests() {
    return this.http.get<DonorNearbyRequestDto[]>(`${this.api}/nearby-requests`);
  }

  updateAvailability(isAvailable: boolean) {
    return this.http.put(`${this.api}/availability`, isAvailable);
  }

  getDonationHistory() {
    return this.http.get<any[]>(`${this.api}/my-history`);
  }
}