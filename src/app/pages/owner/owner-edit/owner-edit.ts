import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

import { PropertyService } from '../../../core/services/property.service';
import { PropertyFull } from '../../../models/property.models';

@Component({
  selector: 'app-owner-edit',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './owner-edit.html',
  styleUrls: ['./owner-edit.scss']
})
export class OwnerEdit implements OnInit{
  propertyId!: number;
  form!: FormGroup;
  property!: PropertyFull;

  constructor(
    private fb: FormBuilder,
    private api: PropertyService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.propertyId = Number(this.route.snapshot.paramMap.get('id'));
    this.form = this.fb.group({
      title: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      pricePerNight: [0, Validators.required],
      propertyType: ['', Validators.required],
      maxGuests: [1, Validators.required],
      bedrooms: [1, Validators.required],
      bathrooms: [1, Validators.required]
    });

    // FETCH property and pre-fill form
    this.api.getById(this.propertyId).subscribe(p => {
      this.property = p;
      this.form.patchValue({
        title: p.title,
        city: p.city,
        country: p.country,
        pricePerNight: p.pricePerNight,
        propertyType: p.propertyType,
        maxGuests: p.maxGuests,
        bedrooms: p.bedrooms,
        bathrooms: p.bathrooms
      });
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    const updatedData = {
      ...this.form.value,
      description: this.property.description ?? '',
      address: this.property.address ?? '',
      latitude: this.property.latitude,
      longitude: this.property.longitude,
      amenityIds: this.property.propertyAmenities?.map(pa => pa.amenityId) ?? []
    };

    this.api.update(this.propertyId, updatedData).subscribe(() => {
      alert('Property updated successfully');
      this.router.navigate(['/owner']);
    }, err => {
      console.error('Update failed:', err);
      alert('Failed to update property');
    });
  }
}