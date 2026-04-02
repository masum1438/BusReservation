import { CommonModule } from "@angular/common";
import { Component, OnInit, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { UserBookingService } from "../../core/services/api.services";
import { DateUtils } from "../../core/utils/date-utils";
import { UserBookingDto } from "../../shared/models/models";


@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.scss'
})
export class MyBookings implements OnInit {
  bookings   = signal<UserBookingDto[]>([]);
  loading    = signal(true);
  cancelling = signal<string | null>(null);
  fmt        = DateUtils;

  constructor(private svc: UserBookingService) {}

  ngOnInit() {
    this.svc.getMyBookings().subscribe({
      next: b  => { this.bookings.set(b); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  cancel(b: UserBookingDto) {
    if (!confirm('Are you sure you want to cancel this booking?')) return;
    this.cancelling.set(b.ticketId);
    this.svc.cancelBooking(b.ticketId).subscribe({
      next: () => {
        this.cancelling.set(null);
        this.bookings.update(list =>
          list.map(x => x.ticketId === b.ticketId ? { ...x, status: 'Cancelled' } : x)
        );
      },
      error: () => this.cancelling.set(null)
    });
  }

  statusColor(s: string): string {
    return s === 'Confirmed' ? '#059669' : s === 'Cancelled' ? '#dc2626' : '#b45309';
  }
  statusBg(s: string): string {
    return s === 'Confirmed' ? 'rgba(16,185,129,.06)'
         : s === 'Cancelled' ? 'rgba(239,68,68,.06)'
         : 'rgba(245,158,11,.06)';
  }
}

