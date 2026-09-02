export interface AuthResponse {
  token: string;
  fullName: string;
  email: string;
  role: string;
}

export interface RegisterDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface Bus {
  busId: number;
  busNumber: string;
  busName: string;
  busType: string;
  totalSeats: number;
  isActive: boolean;
}

export interface Route {
  routeId: number;
  origin: string;
  destination: string;
  distanceKm: number;
  isActive: boolean;
}

export interface Schedule {
  scheduleId: number;
  busId: number;
  busName: string;
  busNumber: string;
  busType: string;
  routeId: number;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  pricePerSeat: number;
  status: string;
  availableSeats: number;
}

export interface Seat {
  seatId: number;
  seatNumber: string;
  seatType: string;
  deck: string;
  isBooked: boolean;
}

export interface Passenger {
  seatId: number;
  passengerName: string;
  passengerAge: number;
  passengerGender: string;
}

export interface CreateBookingDto {
  scheduleId: number;
  passengers: Passenger[];
}

export interface BookingResponse {
  bookingId: number;
  pnrNumber: string;
  bookingStatus: string;
  totalAmount: number;
  bookingDate: string;
  origin: string;
  destination: string;
  departureTime: string;
  busName: string;
  seats: PassengerSeat[];
}

export interface PassengerSeat {
  seatNumber: string;
  passengerName: string;
  passengerAge: number;
  passengerGender: string;
}

export interface PaymentResponse {
  paymentId: number;
  transactionId: string;
  paymentStatus: string;
  amount: number;
  paymentDate: string;
  razorpayOrderId: string;
  razorpayKeyId: string;
}

export interface DashboardStats {
  totalUsers: number;
  totalBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  totalRevenue: number;
  totalBuses: number;
  totalRoutes: number;
  totalSchedules: number;
  recentBookings: any[];
}
