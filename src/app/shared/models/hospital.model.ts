export interface HospitalDto {
  id: string;
  name: string;
  city?: string;
  address?: string;
  latitude: number;
  longitude: number;
  status: number;
  statusLabel: string;
  createdAt: string;
  reviewedAt?: string;
  rejectionReason?: string;
  totalBloodRequests: number;
   licenseDocumentUrl?: string;

}