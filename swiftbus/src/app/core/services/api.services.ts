import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bus, Route, Schedule, Seat, BookingResponse, CreateBookingDto, PaymentResponse, DashboardStats } from '../models/models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BusService {
  private api = `${environment.apiUrl}/bus`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Bus[]> { return this.http.get<Bus[]>(this.api); }
  getById(id: number): Observable<Bus> { return this.http.get<Bus>(`${this.api}/${id}`); }
  getSeats(busId: number, scheduleId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.api}/${busId}/seats?scheduleId=${scheduleId}`);
  }
  create(dto: any): Observable<Bus> { return this.http.post<Bus>(this.api, dto); }
  update(id: number, dto: any): Observable<any> { return this.http.put(`${this.api}/${id}`, dto); }
  deactivate(id: number): Observable<any> { return this.http.delete(`${this.api}/${id}`); }
  addSeats(busId: number, seats: any[]): Observable<any> { return this.http.post(`${this.api}/${busId}/seats`, seats); }
}

@Injectable({ providedIn: 'root' })
export class RouteService {
  private api = `${environment.apiUrl}/route`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Route[]> { return this.http.get<Route[]>(this.api); }
  getById(id: number): Observable<Route> { return this.http.get<Route>(`${this.api}/${id}`); }
  create(dto: any): Observable<Route> { return this.http.post<Route>(this.api, dto); }
  update(id: number, dto: any): Observable<any> { return this.http.put(`${this.api}/${id}`, dto); }
  deactivate(id: number): Observable<any> { return this.http.delete(`${this.api}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class ScheduleService {
  private api = `${environment.apiUrl}/schedule`;
  constructor(private http: HttpClient) {}

  search(origin: string, destination: string, travelDate: string): Observable<Schedule[]> {
    const params = new HttpParams().set('origin', origin).set('destination', destination).set('travelDate', travelDate);
    return this.http.get<Schedule[]>(`${this.api}/search`, { params });
  }
  getAll(): Observable<Schedule[]> { return this.http.get<Schedule[]>(this.api); }
  getById(id: number): Observable<Schedule> { return this.http.get<Schedule>(`${this.api}/${id}`); }
  create(dto: any): Observable<Schedule> { return this.http.post<Schedule>(this.api, dto); }
  updateStatus(id: number, status: string): Observable<any> { return this.http.patch(`${this.api}/${id}/status`, JSON.stringify(status), { headers: { 'Content-Type': 'application/json' } }); }
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  private api = `${environment.apiUrl}/booking`;
  constructor(private http: HttpClient) {}

  create(dto: CreateBookingDto): Observable<BookingResponse> { return this.http.post<BookingResponse>(this.api, dto); }
  getMyBookings(): Observable<BookingResponse[]> { return this.http.get<BookingResponse[]>(`${this.api}/my`); }
  getByPNR(pnr: string): Observable<BookingResponse> { return this.http.get<BookingResponse>(`${this.api}/pnr/${pnr}`); }
  cancel(id: number): Observable<any> { return this.http.patch(`${this.api}/${id}/cancel`, {}); }
  getAll(): Observable<BookingResponse[]> { return this.http.get<BookingResponse[]>(this.api); }
  downloadTicket(id: number): Observable<Blob> {
    return this.http.get(`${this.api}/${id}/download-ticket`, { responseType: 'blob' });
  }
}

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private api = `${environment.apiUrl}/payment`;
  constructor(private http: HttpClient) {}

  initiate(bookingId: number, paymentMethod: string): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${this.api}/initiate`, { bookingId, paymentMethod });
  }
  confirm(dto: { razorpayOrderId: string; razorpayPaymentId: string; razorpaySignature: string }): Observable<any> {
    return this.http.post(`${this.api}/confirm`, dto);
  }
  refund(bookingId: number): Observable<any> { return this.http.post(`${this.api}/refund/${bookingId}`, {}); }
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = `${environment.apiUrl}/admin`;
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardStats> { return this.http.get<DashboardStats>(`${this.api}/dashboard`); }
}
