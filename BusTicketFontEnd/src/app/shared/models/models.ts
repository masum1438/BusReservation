// ── Auth ──────────────────────────────────────────────────────────────────────
export interface LoginDto { email: string; password: string; }
export interface RegisterDto { username: string; email: string; password: string; fullName: string; mobileNumber: string; }
export interface ChangePasswordDto { currentPassword: string; newPassword: string; }
export interface RefreshTokenDto { refreshToken: string; }

export interface UserDto {
  id: string; username: string; email: string;
  fullName: string; mobileNumber: string; role: string;
}
export interface AuthResponse {
  isSuccess: boolean; message: string;
  token?: string; refreshToken?: string; user?: UserDto;
}

// ── Bus ───────────────────────────────────────────────────────────────────────
export interface BusCreateDto {
  busNumber: string; companyName: string; busName: string;
  totalSeats: number; type: number;
}
export interface BusResponseDto {
  id: string; busNumber: string; companyName: string;
  busName: string; totalSeats: number; type: string; createdAt: string;
}

// ── Route ─────────────────────────────────────────────────────────────────────
export interface RouteCreateDto {
  fromCity: string; toCity: string; basePrice: number; distance: number;
}
export interface RouteResponseDto {
  id: string; fromCity: string; toCity: string; basePrice: number; distance: number;
}

// ── Schedule ──────────────────────────────────────────────────────────────────
export interface ScheduleCreateDto {
  busId: string; routeId: string;
  departureTime: string; arrivalTime: string; price: number;
}
export interface ScheduleResponseDto {
  id: string; busName: string; companyName: string;
  fromCity: string; toCity: string;
  departureTime: string; arrivalTime: string;
  price: number; availableSeats: number;
}

// ── Search ────────────────────────────────────────────────────────────────────
export interface AvailableBusDto {
  busScheduleId: string; companyName: string; busName: string;
  startTime: string; arrivalTime: string;
  totalSeats: number; seatsLeft: number; price: number;
}

// ── Seat ──────────────────────────────────────────────────────────────────────
export interface SeatDto {
  seatId: string; seatNumber: string;
  rowNumber: number; columnNumber: number;
  status: string; isAvailable: boolean;
}
export interface SeatPlanDto {
  busScheduleId: string; busName: string;
  fromCity: string; toCity: string;
  departureTime: string; arrivalTime: string;
  seats: SeatDto[];
}
export interface SeatLockRequestDto {
  busScheduleId: string; seatNumbers: string[]; lockDurationMinutes: number;
}
export interface SeatLockResponseDto {
  isSuccess: boolean; message: string;
  lockedSeats: LockedSeatDto[]; expiresAt: string;
}
export interface LockedSeatDto { seatNumber: string; lockId: string; expiresAt: string; }
export interface SeatReleaseRequestDto { lockIds: string[]; }

// ── Booking ───────────────────────────────────────────────────────────────────
export interface BookSeatInputDto {
  busScheduleId: string; seatId: string;
  passengerName: string; mobileNumber: string; email: string;
  boardingPoint: string; droppingPoint: string;
}
export interface BookSeatResultDto {
  isSuccess: boolean; message: string;
  ticketId: string; ticketNumber: string; totalAmount: number;
}
export interface UserBookingDto {
  ticketId: string; ticketNumber: string;
  busName: string; companyName: string;
  fromCity: string; toCity: string;
  departureTime: string; arrivalTime: string;
  seatNumber: string; price: number; status: string;
  bookingDate: string; paymentDate?: string;
}
export interface TicketDetailDto {
  ticketId: string; ticketNumber: string;
  passengerName: string; mobileNumber: string; email: string;
  busName: string; companyName: string;
  fromCity: string; toCity: string;
  departureTime: string; arrivalTime: string;
  seatNumber: string; price: number; status: string;
  bookingDate: string; boardingPoint: string; droppingPoint: string;
}

// ── Admin Booking ─────────────────────────────────────────────────────────────
export interface BookingReportDto {
  ticketId: string; ticketNumber: string;
  passengerName: string; mobileNumber: string;
  busName: string; fromCity: string; toCity: string;
  departureTime: string; seatNumber: string;
  price: number; status: string; bookingDate: string; paymentDate?: string;
}
export interface BookingFilterDto {
  fromDate?: string; toDate?: string; busName?: string;
  status?: string; ticketNumber?: string; page: number; pageSize: number;
}
export interface BookingStatisticsDto {
  totalBookings: number; totalRevenue: number;
  confirmedBookings: number; cancelledBookings: number; pendingBookings: number;
  averageTicketPrice: number;
  bookingsByBus: Record<string, number>;
  bookingsByRoute: Record<string, number>;
}
