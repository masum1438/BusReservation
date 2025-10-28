import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ApiService } from "./api.service";

@Injectable({ providedIn: 'root' })
export class BookingService {
  constructor(private api: ApiService) {}

  getSeatPlan(busScheduleId: string): Observable<any> {
    return this.api.getSeatPlan(busScheduleId);
  }

  bookSeat(payload: any): Observable<any> {
    return this.api.bookSeat(payload);
  }
}