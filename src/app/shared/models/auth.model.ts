export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userId: string;
  fullName: string;
  email: string;
  role: string;
  token: string;
}

export interface RegisterUserRequest {
  fullName: string;
  email: string;
  password: string;
  phone?: string;
  age?: number;
  bloodType?: number;
  role: number;
  latitude: number;
  longitude: number;
}

export interface RegisterHospitalRequest {
  email: string;
  password: string;
  hospitalName: string;
  government: string;
  city: string;
  addressDetail: string;
  latitude: number;
  longitude: number;
  hotline: string;
  licenseDocumentPath: File;
}

export interface RegisterResponse {
  userId: string;
  fullName: string;
  token: string;
}
