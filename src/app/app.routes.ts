import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { DashboardAdmin } from './features/dashboard-admin/dashboard-admin';
import { RequestFormComponent } from './features/request-form/request-form';
import { RequestDetailComponent } from './features/request-detail/request-detail';
import { DashboardUser } from './features/dashboard-User/dashboard-User';
import { DonationRequests } from './features/donation-requests/donation-requests';
import { About } from './features/about/about';
import { Login } from './features/login/login';
import { Register } from './features/register/register';
import { AiPipeline } from './features/ai-pipeline/ai-pipeline';
import { Chatbot } from './shared/components/chatbot/chatbot';
import { BloodBanks } from './features/blood-banks/blood-banks';
import { DashboardHospital } from './features/dashboard-hospital/dashboard-hospital';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  { path: 'dashboard-Admin', component: DashboardAdmin },
  { path: 'request-form', component: RequestFormComponent },
  { path: 'request-detail', component: RequestDetailComponent },
  { path: 'dashboard-User', component: DashboardUser },
  { path: 'donation-requests', component: DonationRequests },
  { path: 'about', component: About },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'AiPipeline', component: AiPipeline },
  { path: 'chatbot', component: Chatbot },
  { path: 'Blood-Banks', component: BloodBanks },
  { path: 'dashboard/hospital', component: DashboardHospital },
  { path: '**', redirectTo: '' },
 
];
