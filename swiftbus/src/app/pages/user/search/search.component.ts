import { Component, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ScheduleService, BusService, BookingService, PaymentService } from '../../../core/services/api.services';
import { Schedule, Seat, Passenger } from '../../../core/models/models';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './search.component.html'
})
export class SearchComponent {
  searchForm: FormGroup;
  schedules: Schedule[] = [];
  searching = false;
  searched = false;

  selectedSchedule: Schedule | null = null;
  seats: Seat[] = [];
  selectedSeats: Seat[] = [];
  seatCount = 1;
  passengers: Passenger[] = [];
  showSeatCountModal = false;
  showPassengerModal = false;
  showPaymentModal = false;

  bookingId = 0;
  bookingLoading = false;
  paymentLoading = false;

  successMessage = '';
  errorMessage = '';

  get user() { return this.auth.currentUser; }
  get isSleeper() { return this.selectedSchedule?.busType === 'Sleeper'; }
  get availableCount() { return this.seats.filter(s => !s.isBooked).length; }
  get today() { return new Date().toISOString().split('T')[0]; }
  decrementSeat() { if (this.seatCount > 1) this.seatCount--; }
  incrementSeat() { if (this.seatCount < this.availableCount) this.seatCount++; }

  constructor(
    private fb: FormBuilder,
    public auth: AuthService,
    private scheduleService: ScheduleService,
    private busService: BusService,
    private bookingService: BookingService,
    private paymentService: PaymentService,
    private router: Router,
    private ngZone: NgZone
  ) {
    this.searchForm = this.fb.group({
      origin: ['', Validators.required],
      destination: ['', Validators.required],
      travelDate: ['', Validators.required]
    });
  }

  search() {
    if (this.searchForm.invalid) return;
    this.searching = true;
    this.searched = false;
    const { origin, destination, travelDate } = this.searchForm.value;
    this.scheduleService.search(origin, destination, travelDate).subscribe({
      next: (data) => { this.schedules = data; this.searching = false; this.searched = true; },
      error: () => { this.searching = false; this.searched = true; }
    });
  }

  selectSchedule(schedule: Schedule) {
    this.selectedSchedule = schedule;
    this.selectedSeats = [];
    this.seatCount = 1;
    this.errorMessage = '';
    this.busService.getSeats(schedule.busId, schedule.scheduleId).subscribe({
      next: (seats) => {
        this.seats = seats;
        this.showSeatCountModal = true;
      },
      error: () => {
        this.errorMessage = 'Failed to load seats. Please try again.';
      }
    });
  }

  proceedToPassengers() {
    this.errorMessage = '';
    if (this.seatCount < 1) return;
    const available = this.seats.filter(s => !s.isBooked);
    if (available.length === 0) {
      this.errorMessage = 'No available seats found.'; return;
    }
    this.selectedSeats = available.slice(0, this.seatCount);
    if (this.selectedSeats.length < this.seatCount) {
      this.errorMessage = `Only ${available.length} seat(s) available.`; return;
    }
    this.passengers = this.selectedSeats.map(s => ({
      seatId: s.seatId, passengerName: '', passengerAge: 0, passengerGender: 'Male'
    }));
    this.showSeatCountModal = false;
    this.errorMessage = '';
    this.showPassengerModal = true;
  }

  confirmBooking() {
    if (this.passengers.some(p => !p.passengerName.trim() || p.passengerAge < 1)) {
      this.errorMessage = 'Please fill in all passenger details.'; return;
    }
    this.bookingLoading = true;
    this.errorMessage = '';
    this.bookingService.create({
      scheduleId: this.selectedSchedule!.scheduleId,
      passengers: this.passengers
    }).subscribe({
      next: (res) => {
        this.bookingId = res.bookingId;
        this.bookingLoading = false;
        this.showPassengerModal = false;
        this.errorMessage = '';
        this.showPaymentModal = true;
      },
      error: (err) => {
        this.bookingLoading = false;
        this.errorMessage = err.error?.message || err.error || 'Booking failed. Please try again.';
      }
    });
  }

  confirmPayment() {
    this.paymentLoading = true;
    this.errorMessage = '';
    this.paymentService.initiate(this.bookingId, 'Card').subscribe({
      next: (res) => {
        this.paymentLoading = false;
        this.showPaymentModal = false;
        const options = {
          key: res.razorpayKeyId,
          amount: res.amount * 100,
          currency: 'INR',
          name: 'SwiftBus',
          description: 'Bus Ticket Booking',
          image: '',
          order_id: res.razorpayOrderId,
          handler: (response: any) => {
            this.paymentService.confirm({
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature
            }).subscribe({
              next: () => {
                this.ngZone.run(() => this.router.navigate(['/user/bookings']));
              },
              error: () => { this.errorMessage = 'Payment verification failed.'; }
            });
          },
          prefill: {
            name: this.user?.fullName,
            email: this.user?.email
          },
          theme: { color: '#6c63ff' },
          modal: {
            ondismiss: () => {
              this.errorMessage = 'Payment cancelled.';
            }
          }
        };
        const rzp = new (window as any).Razorpay(options);
        rzp.on('payment.failed', (response: any) => {
          this.errorMessage = `Payment failed: ${response.error.description}`;
        });
        rzp.open();
      },
      error: () => { this.paymentLoading = false; this.errorMessage = 'Could not initiate payment.'; }
    });
  }

  logout() { this.auth.logout(); }
}
