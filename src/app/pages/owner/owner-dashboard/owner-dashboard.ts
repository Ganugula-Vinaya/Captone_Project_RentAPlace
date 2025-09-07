import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule, Router } from '@angular/router';
import { PropertyService } from '../../../core/services/property.service';
import { PropertyFull } from '../../../models/property.models';
import { MessageService } from '../../../core/services/message.service';
import { Message } from '../../../models/message.models';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../../core/services/auth.service';
import { ReservationService, ReservationDto } from '../../../core/services/reservation.service'; 

@Component({
  selector: 'app-owner-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, RouterModule, FormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './owner-dashboard.html',
  styleUrls: ['./owner-dashboard.scss']
})
export class OwnerDashboard implements OnInit {
  list: PropertyFull[] = [];
  messages: { [propertyId: number]: Message[] } = {};
  replyMessages: { [messageId: number]: string } = {}; //Added: stores reply for each message
  loading = false;
  selectedPropertyId: number | null = null;
  selectedProperty: PropertyFull | null = null;

  //store reservations for owner
  reservations: ReservationDto[] = [];

  constructor(private api: PropertyService, private messagesApi: MessageService,private router: Router,public auth: AuthService,private reservationApi: ReservationService) {}

  ngOnInit() {
    this.refresh();
    this.loadReservations();
  }

  refresh() {
    this.loading = true;
    this.api.mine().subscribe({
      next: res => {
        this.list = res;
        // Fetch messages for each property
        this.list.forEach(prop => {
          this.messagesApi.getByProperty(prop.id).subscribe({
            next: msgs => this.messages[prop.id] = msgs,
            error: _ => this.messages[prop.id] = []
          });
        });
        this.loading = false;
      },
      error: _ => (this.loading = false)
    });
  }

  //load owner reservations from backend
  loadReservations() { 
    this.reservationApi.getOwnerReservations().subscribe({
      next: res => this.reservations = res,
      error: err => console.error('Failed to load reservations', err)
    });
  }

  // Edit property
  edit(prop: PropertyFull) {
    // Navigate to an edit page where you can pre-fill the property details
    this.router.navigate(['/owner/edit', prop.id]);
  }

  // Delete property
  delete(id: number) {
    if (!confirm('Are you sure you want to delete this property?')) return;

    this.api.delete(id).subscribe({
      next: () => {
        // Remove the deleted property from the list
        this.list = this.list.filter(p => p.id !== id);
        alert('Property deleted successfully');
      },
      error: (err) => {
        console.error('Delete failed', err);
        alert('Failed to delete property');
      }
    });
  }

  //Added: open messages modal
  openMessages(propertyId: number) {
    this.selectedPropertyId = propertyId;

    this.selectedProperty = this.list.find(p => p.id === propertyId) ?? null;

    // Fetch messages if not already fetched
    if (!this.messages[propertyId]) {
      this.messagesApi.getByProperty(propertyId).subscribe({
        next: msgs => this.messages[propertyId] = msgs,
        error: _ => this.messages[propertyId] = []
      });
    }
  }

  //Added: close messages modal
  closeMessages() {
    this.selectedPropertyId = null;
     this.selectedProperty = null;
  }



  //Send reply to renter
  sendReply(message: Message, propertyId: number | null, messageId: number) {
  if (!this.replyMessages[messageId]?.trim() || propertyId === null) return;

  // Determine the receiverId: the OTHER user in this message
  const currentUserId = this.auth.userId; // owner ID
  const receiverId = message.senderId === currentUserId ? message.receiverId : message.senderId;

  const payload = {
    propertyId: propertyId,
    receiverId: receiverId,
    content: this.replyMessages[messageId]
  };

  this.messagesApi.send(payload).subscribe({
    next: res => {
      this.replyMessages[messageId] = ''; // clear textarea
      // Refresh messages for this property
      this.messagesApi.getByProperty(propertyId).subscribe(msgs => this.messages[propertyId] = msgs);
    },
    error: err => alert('Failed to send reply')
  });
  }

  //confirm a reservation
  confirmReservation(reservationId: number) { 
    this.reservationApi.confirm(reservationId).subscribe({
      next: _ => this.loadReservations(), // reload reservations after confirm
      error: err => alert('Failed to confirm reservation')
    });
  }

  // Added: cancel a reservation
  cancelReservation(reservationId: number) { 
    this.reservationApi.cancel(reservationId).subscribe({
      next: _ => this.loadReservations(), // reload reservations after cancel
      error: err => alert('Failed to cancel reservation')
    });
  }
}
