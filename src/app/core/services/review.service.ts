import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Review } from '../../models/review.models';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private api = `${environment.apiBase}/api/reviews`;

  constructor(private http: HttpClient) {}

  getByProperty(id: number) {
    return this.http.get<Review[]>(`${this.api}/property/${id}`);
  }

  create(dto: { propertyId: number, rating: number, comment?: string }) {
    return this.http.post(`${this.api}`, dto);
  }
}
