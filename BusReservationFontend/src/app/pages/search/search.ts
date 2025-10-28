import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { ApiService } from '../../services/api.service';

interface BusResult {
  id: string;
  busName: string;
  from: string;
  to: string;
  time: string;
  arrivalTime: string;
  journeyDate: string;
  availableSeats: number;
  price: number;
}

@Component({
  selector: 'app-search',
  standalone: true,
  templateUrl: './search.html',
  styleUrls: ['./search.css'],
  imports: [FormsModule, CommonModule, RouterModule],
  providers: [DatePipe]
})
export class Search {

  from = '';
  to = '';
  journeyDate = '';
  results: BusResult[] = [];
  loading = false;
  errorMessage = '';
  submitted = false;

  constructor(private api: ApiService, private router: Router, private datePipe: DatePipe) {}

  private formatToBDT(time: string): string {
    if (!time) return '';
    const today = new Date().toISOString().split('T')[0];
    const bdTime = new Date(`${today}T${time}`);

    return new Intl.DateTimeFormat('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
      timeZone: 'Asia/Dhaka'
    }).format(bdTime);
  }

  searchBuses(): void {
    this.submitted = true;

    if (!this.from || !this.to) {
      this.errorMessage = 'Please enter both From and To.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.results = [];

    const formattedDate = this.journeyDate
      ? this.datePipe.transform(this.journeyDate, 'yyyy-MM-dd')
      : '';

    this.api.search(this.from, this.to, formattedDate!).subscribe({
      next: (res: any[]) => {
        this.results = (res || []).map(r => ({
          id: r.busScheduleId,
          busName: r.busName ?? r.companyName,
          from: r.from,
          to: r.to,
          time: this.formatToBDT(r.startTime),
          arrivalTime: this.formatToBDT(r.arrivalTime),
          journeyDate: this.datePipe.transform(r.journeyDate, 'dd MMM yyyy')!, // ✅ only date
          availableSeats: r.seatsLeft,
          price: r.price,
        }));

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'No buses found.';
        this.loading = false;
      }
    });
  }

  viewSeats(scheduleId: string) {
    this.router.navigate(['/seat-plan', scheduleId]);
  }
}
