import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { BookingService } from '../../../core/services/api.services';
import { BookingResponse } from '../../../core/models/models';

@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-bookings.component.html'
})
export class AdminBookingsComponent implements OnInit {
  bookings: BookingResponse[] = [];
  loading = true;
  get user() { return this.auth.currentUser; }

  constructor(public auth: AuthService, private bookingService: BookingService) {}

  ngOnInit() {
    this.bookingService.getAll().subscribe({
      next: (data) => { this.bookings = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  logout() { this.auth.logout(); }
}
