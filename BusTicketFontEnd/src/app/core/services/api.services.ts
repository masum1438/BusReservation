import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AvailableBusDto, SeatPlanDto, BookSeatInputDto, BookSeatResultDto, SeatLockRequestDto, SeatLockResponseDto, SeatReleaseRequestDto, UserBookingDto, TicketDetailDto, BusResponseDto, BusCreateDto, RouteResponseDto, RouteCreateDto, ScheduleResponseDto, ScheduleCreateDto, BookingFilterDto, BookingReportDto, BookingStatisticsDto } from '../../shared/models/models';


const API = environment.apiUrl;

function toIsoDate(date: string | Date | null | undefined): string | null {
  if (!date) return null;
  return new Date(date).toISOString();
}

@Injectable({ providedIn: 'root' })
export class SearchService {
  constructor(private http: HttpClient) {}

  searchBuses(from: string, to: string, date: string): Observable<AvailableBusDto[]> {
    const params = new HttpParams()
      .set('from', from)
      .set('to', to)
      .set('journeyDate', toIsoDate(date)!);

    return this.http.get<AvailableBusDto[]>(
      `${API}/Search`,
      { params }
    );
  }
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  constructor(private http: HttpClient) {}

  getSeatPlan(scheduleId: string): Observable<SeatPlanDto> {
    return this.http.get<SeatPlanDto>(
      `${API}/Booking/seat-plan/${scheduleId}`
    );
  }

  bookSeat(dto: BookSeatInputDto): Observable<BookSeatResultDto> {
    return this.http.post<BookSeatResultDto>(
      `${API}/Booking/book`,
      dto
    );
  }
}

@Injectable({ providedIn: 'root' })
export class SeatLockService {
  constructor(private http: HttpClient) {}

  lockSeats(dto: SeatLockRequestDto): Observable<SeatLockResponseDto> {
    return this.http.post<SeatLockResponseDto>(
      `${API}/Seats/lock`,
      dto
    );
  }

  releaseSeats(dto: SeatReleaseRequestDto): Observable<any> {
    return this.http.post(
      `${API}/Seats/release`,
      dto
    );
  }
}

@Injectable({ providedIn: 'root' })
export class UserBookingService {
  constructor(private http: HttpClient) {}

  getMyBookings(): Observable<UserBookingDto[]> {
    return this.http.get<UserBookingDto[]>(
      `${API}/User/bookings`
    );
  }

  getTicketById(id: string): Observable<TicketDetailDto> {
    return this.http.get<TicketDetailDto>(
      `${API}/User/bookings/${id}`
    );
  }

  getTicketByNumber(number: string): Observable<TicketDetailDto> {
    return this.http.get<TicketDetailDto>(
      `${API}/User/bookings/number/${number}`
    );
  }

  cancelBooking(id: string): Observable<any> {
    return this.http.post(
      `${API}/User/bookings/cancel/${id}`,
      {}
    );
  }
}

@Injectable({ providedIn: 'root' })
export class AdminBusService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<BusResponseDto[]> {
    return this.http.get<BusResponseDto[]>(
      `${API}/Admin/buses`
    );
  }

  get(id: string): Observable<BusResponseDto> {
    return this.http.get<BusResponseDto>(
      `${API}/Admin/buses/${id}`
    );
  }

  create(dto: BusCreateDto): Observable<BusResponseDto> {
    return this.http.post<BusResponseDto>(
      `${API}/Admin/buses`,
      dto
    );
  }

  update(id: string, dto: BusCreateDto): Observable<BusResponseDto> {
    return this.http.put<BusResponseDto>(
      `${API}/Admin/buses/${id}`,
      dto
    );
  }

  delete(id: string): Observable<any> {
    return this.http.delete(
      `${API}/Admin/buses/${id}`
    );
  }
}

@Injectable({ providedIn: 'root' })
export class AdminRouteService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<RouteResponseDto[]> {
    return this.http.get<RouteResponseDto[]>(
      `${API}/Admin/routes`
    );
  }

  get(id: string): Observable<RouteResponseDto> {
    return this.http.get<RouteResponseDto>(
      `${API}/Admin/routes/${id}`
    );
  }

  create(dto: RouteCreateDto): Observable<RouteResponseDto> {
    return this.http.post<RouteResponseDto>(
      `${API}/Admin/routes`,
      dto
    );
  }

  update(id: string, dto: RouteCreateDto): Observable<RouteResponseDto> {
    return this.http.put<RouteResponseDto>(
      `${API}/Admin/routes/${id}`,
      dto
    );
  }

  delete(id: string): Observable<any> {
    return this.http.delete(
      `${API}/Admin/routes/${id}`
    );
  }
}

@Injectable({ providedIn: 'root' })
export class AdminScheduleService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<ScheduleResponseDto[]> {
    return this.http.get<ScheduleResponseDto[]>(
      `${API}/Admin/schedules`
    );
  }

  create(dto: ScheduleCreateDto): Observable<ScheduleResponseDto> {
    return this.http.post<ScheduleResponseDto>(
      `${API}/Admin/schedules`,
      dto
    );
  }

  update(id: string, dto: ScheduleCreateDto): Observable<ScheduleResponseDto> {
    return this.http.put<ScheduleResponseDto>(
      `${API}/Admin/schedules/${id}`,
      dto
    );
  }

  delete(id: string): Observable<any> {
    return this.http.delete(
      `${API}/Admin/schedules/${id}`
    );
  }
}

@Injectable({ providedIn: 'root' })
export class AdminBookingService {
  constructor(private http: HttpClient) {}

  getAll(filter: BookingFilterDto): Observable<BookingReportDto[]> {
    let params = new HttpParams()
      .set('page', filter.page)
      .set('pageSize', filter.pageSize);

    if (filter.fromDate)
      params = params.set('fromDate', toIsoDate(filter.fromDate)!);

    if (filter.toDate)
      params = params.set('toDate', toIsoDate(filter.toDate)!);

    if (filter.busName)
      params = params.set('busName', filter.busName);

    if (filter.status)
      params = params.set('status', filter.status);

    if (filter.ticketNumber)
      params = params.set('ticketNumber', filter.ticketNumber);

    return this.http.get<BookingReportDto[]>(
      `${API}/Admin/bookings`,
      { params }
    );
  }

  getStatistics(from?: string, to?: string): Observable<BookingStatisticsDto> {
    let params = new HttpParams();

    if (from)
      params = params.set('from', toIsoDate(from)!);

    if (to)
      params = params.set('to', toIsoDate(to)!);

    return this.http.get<BookingStatisticsDto>(
      `${API}/Admin/bookings/statistics`,
      { params }
    );
  }

  cancel(id: string, reason: string): Observable<any> {
    return this.http.post(
      `${API}/Admin/bookings/${id}/cancel`,
      JSON.stringify(reason),
      {
        headers: {
          'Content-Type': 'application/json'
        }
      }
    );
  }
}