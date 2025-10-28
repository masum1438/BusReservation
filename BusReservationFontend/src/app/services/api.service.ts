import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environment";

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly apiRoot = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  search(from: string, to: string, journeyDate?: string): Observable<any> {
    let url = `${this.apiRoot}/search?from=${from}&to=${to}`;
    if (journeyDate) url += `&journeyDate=${journeyDate}`;
    return this.http.get(url);
  }

  getSeatPlan(busScheduleId: string): Observable<any> {
    return this.http.get(`${this.apiRoot}/booking/${busScheduleId}/seatplan`);
  }

  bookSeat(payload: any): Observable<any> {
    return this.http.post(`${this.apiRoot}/booking/book`, payload);
  }
}