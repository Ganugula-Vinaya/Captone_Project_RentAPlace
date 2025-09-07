import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message } from '../../models/message.models';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private apiUrl = 'http://localhost:5264/api/messages';

  constructor(private http: HttpClient) {}

  send(msg: Partial<Message>): Observable<Message> {
    return this.http.post<Message>(this.apiUrl, msg);
  }

  getByProperty(propertyId: number): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/property/${propertyId}`);
  }

  markAsRead(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/read`, {});
  }
}
