export interface CreateAdminRequest {
  fullName: string;
  email: string;
  password: string;
  phone: string;
}

export interface DashboardStats {
  totalRequests: number;
  activeRequests: number;
  totalDonors: number;
  totalDonations: number;
  totalNotificationsSent: number;
}