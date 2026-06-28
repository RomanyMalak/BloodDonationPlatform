export interface BloodRequestSummaryDto {
  id: string;
  patientName: string;
  requiredBloodType: string;
  urgency: string;
  status: string;
  bloodType?: string;
  hospitalName?: string;
  unitsNeeded: number;
  contactPhone?: string;
  createdAt: string;
  expiresAt?: string;
}

export interface BloodRequestDetailsDto {
  id: string;
  patientName: string;
  patientAge?: number;
  requiredBloodType: string;
  urgency: string;
  status: string;
hospitalName?: string;
  latitude: number;
  longitude: number;
  unitsNeeded: number;
  contactPhone?: string;
  notes?: string;
  createdAt?: string;
  expiresAt?: string;
  approvedAt?: string;
}

export interface CreateBloodRequestDto {
  patientName: string;
  patientAge?: number;
  requiredBloodType?: number;
  urgency: number;
  hospitalId?: string;
  customHospitalName?: string;
  latitude: number;
  longitude: number;
  medicalDocumentUrl?: File | null;
  notes?: string;
  contactPhone?: string;
  unitsNeeded: number;
  expiresAt?: string;
}


export interface DonorNearbyRequestDto {
  id: string;
  requiredBloodType: string;
  urgency: string;
  status: string;
  hospitalName: string;
  distanceKm: number;
  createdAt: string;
}