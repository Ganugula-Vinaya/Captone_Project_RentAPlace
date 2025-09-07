import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';

import { PropertyFull } from '../../models/property.models';
import { PropertyService } from '../../core/services/property.service';
import { ReservationService } from '../../core/services/reservation.service';
import { AuthService } from '../../core/services/auth.service';
import { MessageService } from '../../core/services/message.service';
import { Message } from '../../models/message.models';

import { ReviewService } from '../../core/services/review.service';
import { Review } from '../../models/review.models';

import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatChipsModule,
    MatIconModule
  ],
  templateUrl: './property-detail.html',
  styleUrls: ['./property-detail.scss']
})
export class PropertyDetail implements OnInit {
  id!: number;
  data?: PropertyFull;

  checkIn = '';
  checkOut = '';
  guests = 1;
  msg = '';
  newMessage = '';
  messageStatus = '';


  //Store messages (replies) for this renter
  replies: Message[] = [];
  myReservations: any[] = [];

  // --- Reviews ---
  reviews: Review[] = [];       // all reviews for this property
  myReview?: Review;            // renter's review
  ratingHover = 0;              // star hover state
  ratingSelected = 0;           // star selected state
  comment = '';                 // optional comment


  constructor(
    private route: ActivatedRoute,
    private api: PropertyService,
    private reservations: ReservationService,
    public auth: AuthService,
    private messages: MessageService,
    private reviewApi: ReviewService
  ) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getById(this.id).subscribe(p => {
      this.data = p;
      this.loadReviews();  // <-- load reviews after property loaded
      this.loadMyReservations();
    });

    this.loadMessages();
    
  }

  // --- Load reviews ---
  loadReviews() {
    this.reviewApi.getByProperty(this.id).subscribe(res => {
      this.reviews = res;

      // check if current renter already submitted
      this.myReview = this.reviews.find(r => r.renterId === this.auth.userId);
      if (this.myReview) {
        this.ratingSelected = this.myReview.rating;
        this.comment = this.myReview.comment || '';
      }
    });
  }

  // --- Submit review ---
  submitReview() {
    if (this.ratingSelected <= 0) {
      this.msg = 'Please select a rating';
      return;
    }

    this.reviewApi.create({
      propertyId: this.id,
      rating: this.ratingSelected,
      comment: this.comment
    }).subscribe({
      next: () => {
        this.msg = 'Review submitted successfully';
        this.loadReviews();
      },
      error: err => this.msg = err?.error ?? 'Failed to submit review'
    });
  }

  // --- Star hover + click ---
  setHover(star: number) { this.ratingHover = star; }
  setRating(star: number) { this.ratingSelected = star; }

  // Calculate average rating (rounded to nearest integer for display)
  averageRating(): number {
    if (!this.reviews || this.reviews.length === 0) return 0;
    const total = this.reviews.reduce((sum, r) => sum + r.rating, 0);
    return Math.round(total / this.reviews.length); 
  }

  loadMessages() {
    this.messages.getByProperty(this.id).subscribe(msgs => {
      const userId = this.auth.userId; // current renter
      // show only messages where renter is sender or receiver
      this.replies = msgs.filter(m =>
        m.senderId.toString() === userId || m.receiverId.toString() === userId || m.senderId !== userId
      );
    });
  }
  loadMyReservations() {
    this.reservations.getMy().subscribe(res => {
      this.myReservations = res.filter((r: any) => r.propertyTitle === this.data?.title);
      console.log(this.myReservations);
    });
  }



  book() {
    if (!this.checkIn || !this.checkOut) {
      this.msg = 'Please select check-in and check-out dates';
      return;
    }
    this.reservations
      .create({
        propertyId: this.id,
        checkIn: this.checkIn,
        checkOut: this.checkOut,
        guests: this.guests
      })
      .subscribe({
        next: (res: any) => {
          this.msg = `Reservation ${res.status}. Total price: ₹ ${res.totalPrice}`;
          this.loadMyReservations();
        },
        error: (err: any) => (this.msg = err?.error ?? 'Booking failed')
      });
  }


  sendMessage() {
    if (!this.newMessage.trim() || !this.data) {
      this.messageStatus = 'Message cannot be empty';
      return;
    }

    const payload = {
      propertyId: this.data.id,
      receiverId: this.data.ownerId,
      content: this.newMessage
    };

    this.messages.send(payload).subscribe({
      next: res => {
        this.messageStatus = 'Message sent!';
        this.newMessage = '';
        this.loadMessages(); ///refresh message after sending 
      },
      error: err => {
        this.messageStatus = err?.error ?? 'Failed to send message';
      }
    });
  }

  // method for renter cancel reservation
  cancelReservation(id: number) {
    this.reservations.cancel(id).subscribe({
      next: () => {
        // Refresh reservations after cancelling
        this.loadMyReservations();
        this.msg = 'Reservation cancelled successfully';
      },
      error: err => {
        this.msg = err?.error ?? 'Failed to cancel reservation';
      }
    });
  }

  
}
