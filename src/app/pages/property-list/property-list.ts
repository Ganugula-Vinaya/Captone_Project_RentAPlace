import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

import { PropertyService } from '../../core/services/property.service';
import { PropertyCard } from '../../models/property.models';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule
  ],
  templateUrl: './property-list.html',
  styleUrls: ['./property-list.scss']
})
export class PropertyList implements OnInit {
  items: PropertyCard[] = [];
  total = 0;
  loading = false;

  // simple filters
  city = '';
  country = '';
  propertyType = '';
  guests: number | null = null;

  constructor(private api: PropertyService, private route: ActivatedRoute, private router: Router, public auth: AuthService) {}

  ngOnInit() {
    // read query params for initial search
    this.route.queryParamMap.subscribe(p => {
      this.city = p.get('city') ?? '';
      this.country = p.get('country') ?? '';
      this.propertyType = p.get('propertyType') ?? '';
      this.guests = p.get('guests') ? Number(p.get('guests')) : null;
      this.search();
    });
  }

  search() {
    this.loading = true;
    this.api.search({
      city: this.city || undefined,
      country: this.country || undefined,
      propertyType: this.propertyType || undefined,
      guests: this.guests ?? undefined,
      page: 1,
      pageSize: 24
    }).subscribe({
      next: res => {
        this.items = res.items;
        this.total = res.total;
        this.loading = false;
      },
      error: _ => {
        this.loading = false;
      }
    });
  }

  view(id: number) {
    this.router.navigate(['/properties', id]);
  }


}
