import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ScheduleService, BusService, RouteService } from '../../../core/services/api.services';
import { Schedule, Bus, Route } from '../../../core/models/models';

@Component({
  selector: 'app-schedules',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './schedules.component.html'
})
export class SchedulesComponent implements OnInit {
  schedules: Schedule[] = [];
  buses: Bus[] = [];
  routes: Route[] = [];
  loading = true;
  showModal = false;
  saving = false;
  form = { busId: 0, routeId: 0, departureTime: '', arrivalTime: '', pricePerSeat: 0 };

  get user() { return this.auth.currentUser; }

  constructor(
    public auth: AuthService,
    private scheduleService: ScheduleService,
    private busService: BusService,
    private routeService: RouteService
  ) {}

  ngOnInit() {
    this.scheduleService.getAll().subscribe({ next: (d) => { this.schedules = d; this.loading = false; }, error: () => { this.loading = false; } });
    this.busService.getAll().subscribe(d => this.buses = d.filter(b => b.isActive));
    this.routeService.getAll().subscribe(d => this.routes = d.filter(r => r.isActive));
  }

  save() {
    this.saving = true;
    this.scheduleService.create(this.form).subscribe({
      next: () => {
        this.saving = false; this.showModal = false;
        this.scheduleService.getAll().subscribe(d => this.schedules = d);
      },
      error: () => { this.saving = false; }
    });
  }

  updateStatus(id: number, status: string) {
    this.scheduleService.updateStatus(id, status).subscribe(() => {
      this.scheduleService.getAll().subscribe(d => this.schedules = d);
    });
  }

  logout() { this.auth.logout(); }
}
