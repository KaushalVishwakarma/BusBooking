import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { BookingService } from '../../../core/services/api.services';
import { BookingResponse } from '../../../core/models/models';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './user-dashboard.component.html'
})
export class UserDashboardComponent implements OnInit {
  bookings: BookingResponse[] = [];
  loading = true;

  get user() { return this.auth.currentUser; }
  get totalBookings() { return this.bookings.length; }
  get confirmedBookings() { return this.bookings.filter(b => b.bookingStatus === 'Confirmed').length; }
  get cancelledBookings() { return this.bookings.filter(b => b.bookingStatus === 'Cancelled').length; }
  get totalSpent() { return this.bookings.filter(b => b.bookingStatus === 'Confirmed').reduce((s, b) => s + b.totalAmount, 0); }

  constructor(public auth: AuthService, private bookingService: BookingService) {}

  ngOnInit() {
    this.bookingService.getMyBookings().subscribe({
      next: (data) => { this.bookings = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  downloadTicket(id: number, pnr: string) {
    this.bookingService.downloadTicket(id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Ticket_${pnr}.pdf`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  logout() { this.auth.logout(); }
}
