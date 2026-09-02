import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { BusService } from '../../../core/services/api.services';
import { Bus } from '../../../core/models/models';

@Component({
  selector: 'app-buses',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './buses.component.html'
})
export class BusesComponent implements OnInit {
  buses: Bus[] = [];
  loading = true;
  showModal = false;
  saving = false;
  errorMessage = '';

  form = { busNumber: '', busName: '', busType: 'Seater', totalSeats: 40 };

  get user() { return this.auth.currentUser; }
  get isSleeper() { return this.form.busType === 'Sleeper'; }

  constructor(public auth: AuthService, private busService: BusService) {}

  ngOnInit() { this.load(); }

  load() {
    this.busService.getAll().subscribe({
      next: (data) => { this.buses = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openAdd() {
    this.form = { busNumber: '', busName: '', busType: 'Seater', totalSeats: 40 };
    this.errorMessage = '';
    this.showModal = true;
  }

  onTypeChange() {
    this.form.totalSeats = Math.ceil(this.form.totalSeats / 4) * 4;
  }

  save() {
    this.errorMessage = '';
    if (!this.form.busNumber.trim()) { this.errorMessage = 'Bus number is required.'; return; }
    if (!this.form.busName.trim()) { this.errorMessage = 'Bus name is required.'; return; }
    if (this.isSleeper && this.form.totalSeats % 4 !== 0) {
      this.errorMessage = 'Sleeper buses must have seats in multiples of 4 (e.g. 20, 40).'; return;
    }
    if (!this.isSleeper && this.form.totalSeats % 4 !== 0) {
      this.errorMessage = 'Seater/Semi-Sleeper buses must have seats in multiples of 4.'; return;
    }
    this.saving = true;
    this.busService.create(this.form).subscribe({
      next: () => { this.saving = false; this.showModal = false; this.load(); },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err.status === 409 ? 'Bus number already exists.' : 'Failed to create bus.';
      }
    });
  }

  deactivate(id: number) {
    if (!confirm('Remove this bus?')) return;
    this.busService.deactivate(id).subscribe(() => this.load());
  }

  logout() { this.auth.logout(); }
}
