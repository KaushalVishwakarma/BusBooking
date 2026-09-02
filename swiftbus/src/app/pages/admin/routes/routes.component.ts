import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { RouteService } from '../../../core/services/api.services';
import { Route } from '../../../core/models/models';

@Component({
  selector: 'app-routes',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './routes.component.html'
})
export class RoutesComponent implements OnInit {
  routes: Route[] = [];
  loading = true;
  showModal = false;
  saving = false;
  form = { origin: '', destination: '', distanceKm: 0 };

  get user() { return this.auth.currentUser; }

  constructor(public auth: AuthService, private routeService: RouteService) {}

  ngOnInit() { this.load(); }

  load() {
    this.routeService.getAll().subscribe({
      next: (data) => { this.routes = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  save() {
    this.saving = true;
    this.routeService.create(this.form).subscribe({
      next: () => { this.saving = false; this.showModal = false; this.load(); },
      error: () => { this.saving = false; }
    });
  }

  deactivate(id: number) {
    if (!confirm('Deactivate this route?')) return;
    this.routeService.deactivate(id).subscribe(() => this.load());
  }

  logout() { this.auth.logout(); }
}
