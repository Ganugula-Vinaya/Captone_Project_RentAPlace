import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReservationCreateDto } from '../../models/reservation.models';
import { environment } from '../../../environments/environment';

//interface for owner reservations
export interface ReservationDto {
  id: number;
  propertyTitle: string;
  renterId: string;
  status: string;
  checkIn: string;
  checkOut: string;
  totalPrice: number;
}

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly api = `${environment.apiBase}/api/reservations`;

  constructor(private http: HttpClient) {}

  create(dto: ReservationCreateDto) {
    return this.http.post<{ id: number; status: string; totalPrice: number }>(this.api, dto);
  }

   //fetch reservations for logged-in owner
  getOwnerReservations() { 
    return this.http.get<ReservationDto[]>(`${this.api}/owner`);
  }

  // Added: confirm reservation
  confirm(id: number) { 
    return this.http.post(`${this.api}/${id}/confirm`, {});
  }

  // Added: cancel reservation
  cancel(id: number) { 
    return this.http.post(`${this.api}/${id}/cancel`, {});
  }

  getMy() {
    return this.http.get<any[]>(`${this.api}/mine`); 
  }
}
