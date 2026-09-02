import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { BookingService } from '../../../core/services/api.services';
import { BookingResponse } from '../../../core/models/models';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './bookings.component.html'
})
export class BookingsComponent implements OnInit {
  bookings: BookingResponse[] = [];
  loading = true;
  cancellingId = 0;

  get user() { return this.auth.currentUser; }

  constructor(public auth: AuthService, private bookingService: BookingService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.bookingService.getMyBookings().subscribe({
      next: (data) => { this.bookings = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  cancel(id: number) {
    if (!confirm('Are you sure you want to cancel this booking?')) return;
    this.cancellingId = id;
    this.bookingService.cancel(id).subscribe({
      next: () => { this.cancellingId = 0; this.load(); },
      error: () => { this.cancellingId = 0; }
    });
  }

  downloadTicket(id: number, pnr: string) {
    this.bookingService.downloadTicket(id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url; a.download = `Ticket_${pnr}.pdf`;
      a.click(); window.URL.revokeObjectURL(url);
    });
  }

  logout() { this.auth.logout(); }
}
