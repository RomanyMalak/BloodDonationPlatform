import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {StorageService} from './storage.service';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterUserRequest, RegisterResponse, RegisterHospitalRequest } from '../../shared/models/auth.model';


@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = `${environment.apiUrl}`;

    private storage = inject(StorageService);
  constructor(private http: HttpClient) {}

  registerUser(data: RegisterUserRequest) {
    return this.http.post<RegisterResponse>(`${this.api}/Auth/register/User`, data);
  }

  ////// login 
  
  login(data: LoginRequest) {
  return this.http.post<LoginResponse>(`${environment.apiUrl}/Auth/login`, data);
}

saveSession(res: LoginResponse) {
  this.storage.set('token', res.token);
  this.storage.set('userId', res.userId);
  this.storage.set('fullName', res.fullName);
  this.storage.set('role', res.role);
}

  getRole(): string | null {
    return this.storage.get('role');
  }

  ////// Hospital Register
  registerHospital(data: RegisterHospitalRequest) {
    const formData = new FormData();
    formData.append('Email', data.email);
    formData.append('Password', data.password);
    formData.append('HospitalName', data.hospitalName);
    formData.append('Government', data.government);
    formData.append('City', data.city);
    formData.append('AddressDetail', data.addressDetail);
    formData.append('Latitude', data.latitude.toString());
    formData.append('Longitude', data.longitude.toString());
    formData.append('Hotline', data.hotline);
    formData.append('LicenseDocumentPath', data.licenseDocumentPath, data.licenseDocumentPath.name);
    formData.forEach((value, key) => console.log(key, value));

    return this.http.post<any>(`${environment.apiUrl}/Auth/register/Hospital`, formData);
  }
}

