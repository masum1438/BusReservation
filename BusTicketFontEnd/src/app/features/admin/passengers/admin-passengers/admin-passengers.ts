import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, computed } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AdminBookingService } from "../../../../core/services/api.services";
import { BookingReportDto } from "../../../../shared/models/models";


interface PassengerSummary {
  name: string;
  mobile: string;
  bookings: BookingReportDto[];
  confirmedCount: number;   // pre-computed — avoids .filter() in template
  totalSpent: number;
  lastTrip: string;
}

@Component({
  selector: 'app-admin-passengers',
  standalone: true,
  imports: [CommonModule, FormsModule],   // No DatePipe — use formatDate() helper
  templateUrl: './admin-passengers.html',
  styleUrl: './admin-passengers.scss'
  
})
export class AdminPassengers implements OnInit {
  passengers = signal<PassengerSummary[]>([]);
  loading    = signal(true);
  search     = '';
  selected   = signal<PassengerSummary | null>(null);

  filtered = computed(() =>
    this.passengers().filter(p =>
      !this.search ||
      p.name.toLowerCase().includes(this.search.toLowerCase()) ||
      p.mobile.includes(this.search)
    )
  );

  constructor(private svc: AdminBookingService) {}

  ngOnInit() {
    this.svc.getAll({ page: 1, pageSize: 200 }).subscribe({
      next: bookings => {
        const map = new Map<string, PassengerSummary>();

        for (const b of bookings) {
          if (!map.has(b.mobileNumber)) {
            map.set(b.mobileNumber, {
              name:           b.passengerName,
              mobile:         b.mobileNumber,
              bookings:       [],
              confirmedCount: 0,
              totalSpent:     0,
              lastTrip:       b.bookingDate
            });
          }
          const p = map.get(b.mobileNumber)!;
          p.bookings.push(b);

          // Pre-compute confirmed count to avoid => in template
          if (b.status === 'Confirmed') {
            p.confirmedCount++;
            p.totalSpent += b.price;
          }
          if (b.bookingDate > p.lastTrip) {
            p.lastTrip = b.bookingDate;
          }
        }

        this.passengers.set(
          Array.from(map.values()).sort((a, b) => b.bookings.length - a.bookings.length)
        );
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  selectPassenger(p: PassengerSummary) { this.selected.set(p); }

  /**
   * Convert ISO datetime string from backend to readable format.
   * Backend sends UTC ISO strings like "2024-03-24T12:00:00" or "2024-03-24T12:00:00Z".
   * We normalise to UTC and display in a consistent "dd MMM yyyy, HH:mm" format.
   */
  formatDate(iso: string): string {
    if (!iso) return '—';
    try {
      // Ensure we treat the string as UTC (backend sends UTC without Z sometimes)
      const utcStr = iso.endsWith('Z') ? iso : iso + 'Z';
      const d = new Date(utcStr);
      if (isNaN(d.getTime())) return iso;
      return d.toLocaleDateString('en-GB', {
        day: '2-digit', month: 'short', year: 'numeric', timeZone: 'UTC'
      }) + ', ' + d.toLocaleTimeString('en-GB', {
        hour: '2-digit', minute: '2-digit', hour12: false, timeZone: 'UTC'
      });
    } catch {
      return iso;
    }
  }
}
