import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BloodRequestSummaryDto, BloodRequestDetailsDto, CreateBloodRequestDto } from '../../shared/models/blood-request.model';
import { text } from 'stream/consumers';

@Injectable({ providedIn: 'root' })
export class BloodRequestService {
  private api = `${environment.apiUrl}/blood-requests`;

  constructor(private http: HttpClient) {}

  getAvailable() {
    return this.http.get<BloodRequestSummaryDto[]>(`${this.api}`);
  }

  getAll() {
    return this.http.get<BloodRequestSummaryDto[]>(`${this.api}/all`); // ← غيري الـ endpoint لو مختلف
  }

  getById(id: string) {
    return this.http.get<BloodRequestDetailsDto>(`${this.api}/${id}`);
  }

  create(data: CreateBloodRequestDto) {
    const formData = new FormData();
    formData.append('PatientName', data.patientName);
    if (data.patientAge) formData.append('PatientAge', data.patientAge.toString());
    if (data.requiredBloodType !== undefined) formData.append('RequiredBloodType', data.requiredBloodType.toString());
    formData.append('Urgency', data.urgency.toString());
    if (data.hospitalId) formData.append('HospitalId', data.hospitalId);
    if (data.customHospitalName) formData.append('CustomHospitalName', data.customHospitalName);
    formData.append('Latitude', data.latitude.toString());
    formData.append('Longitude', data.longitude.toString());
    formData.append('MedicalDocumentUrl', data.medicalDocumentUrl ? data.medicalDocumentUrl : '');
    if (data.notes) formData.append('Notes', data.notes);
    if (data.contactPhone) formData.append('ContactPhone', data.contactPhone);
    formData.append('UnitsNeeded', data.unitsNeeded.toString());
    if (data.expiresAt) formData.append('ExpiresAt', data.expiresAt);

    return this.http.post<any>(`${this.api}`, formData);
  }

  accept(id: string) {
    return this.http.post(`${this.api}/${id}/acceptBloodRequest`, {} , { responseType : 'text'});
  }

  cancel(id: string) {
    return this.http.put(`${this.api}/${id}/cancelBloodRequest`, {}, { responseType: 'text' });
  }

  complete(id: string, donorId: string, notes?: string) {
    return this.http.post(`${this.api}/${id}/completeBloodRequest`, { donorId, notes });
  }

  getMyRequests() {
    return this.http.get<BloodRequestSummaryDto[]>(`${this.api}/my-requests`);
  }
}