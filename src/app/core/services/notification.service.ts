import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { StorageService } from './storage.service';

export interface NotificationDto {
  id: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  bloodRequestId?: string;
  donorId?: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private api = `${environment.apiUrl}/notifications`;
  private storage = inject(StorageService);
  private http = inject(HttpClient);

  private hubConnection: signalR.HubConnection | null = null;
  
  notifications$ = new BehaviorSubject<NotificationDto[]>([]);
  unreadCount$ = new BehaviorSubject<number>(0);

  getMyNotifications() {
    return this.http.get<NotificationDto[]>(`${this.api}/my`);
  }

  markAsRead(id: string) {
    return this.http.put(`${this.api}/${id}/read`, {});
  }

  markAllAsRead() {
    return this.http.put(`${this.api}/read-all`, {});
  }

  startConnection() {
    const token = this.storage.get('token');
    if (!token) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://bloodddonation.runasp.net/hubs/notifications', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
      const current = this.notifications$.value;
      this.notifications$.next([notification, ...current]);
      this.unreadCount$.next(this.unreadCount$.value + 1);
    });

    this.hubConnection.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR error:', err));
  }

  stopConnection() {
    this.hubConnection?.stop();
  }

  loadNotifications() {
    this.getMyNotifications().subscribe({
      next: (res) => {
        this.notifications$.next(res);
        this.unreadCount$.next(res.filter(n => !n.isRead).length);
      },
      error: (err) => console.error('notifications error:', err)
    });
  }
}