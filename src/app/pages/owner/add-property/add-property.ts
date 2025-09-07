import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

import { PropertyService } from '../../../core/services/property.service';
import { PropertyCreateDto } from '../../../models/property.models';

@Component({
  selector: 'app-add-property',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  templateUrl: './add-property.html',
  styleUrls: ['./add-property.scss']
})
export class AddProperty {
  model: PropertyCreateDto = {
    title: '',
    description: '',
    address: '',
    city: '',
    state: '',
    country: '',
    latitude: undefined,
    longitude: undefined,
    pricePerNight: 1000,
    propertyType: 'Apartment',
    maxGuests: 2,
    bedrooms: 1,
    bathrooms: 1,
    amenityIds: []
  };

  newPropertyId: number | null = null;
  files: File[] = [];
  msg = '';

  //amenities list fetched from backend
  amenitiesList: { id: number; name: string }[] = [];

  constructor(private api: PropertyService, private router: Router) {}

  ngOnInit() {
    // ✅ Fetch amenities from backend
    this.api.getAmenities().subscribe({
      next: (res: any) => this.amenitiesList = res,
      error: (err: any) => this.msg = 'Failed to load amenities'
    });
  }

  create() {
    this.api.create(this.model).subscribe({
      next: (res: any) => {
        this.newPropertyId = res.id;
        // Show success message
        this.msg = 'Property created successfully! Now you can upload images.';

        // Auto-reset the form after adding
        this.model = {
          title: '',
          description: '',
          address: '',
          city: '',
          state: '',
          country: '',
          latitude: undefined,
          longitude: undefined,
          pricePerNight: 1000,
          propertyType: 'Apartment',
          maxGuests: 2,
          bedrooms: 1,
          bathrooms: 1,
          amenityIds: []
        };

        // Optional: Clear message after 3 seconds
        setTimeout(() => this.msg = '', 3000);
      },
      error: (err: any) => (this.msg = err?.error ?? 'Failed to create property')
    });
  }

  fileChanged(e: any) {
    this.files = Array.from(e.target.files) as File[];
  }

  upload() {
    if (!this.newPropertyId) {
      this.msg = 'Create the property first.';
      return;
    }
    if (!this.files.length) {
      this.msg = 'Select one or more images.';
      return;
    }
    this.api.uploadImages(this.newPropertyId, this.files).subscribe({
      next: () => {
        this.msg = 'Images uploaded. Redirecting...';
        setTimeout(() => this.router.navigate(['/owner']), 1000);
      },
      error: (err: any) => (this.msg = err?.error ?? 'Image upload failed')
    });
  }
  getFileUrl(file: File) {
    return file ? URL.createObjectURL(file) : '';
  }
}
