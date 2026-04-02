import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, computed } from "@angular/core";
import { RouterLink } from "@angular/router";
import { AdminBookingService } from "../../../../core/services/api.services";
import { BookingStatisticsDto, BookingReportDto } from "../../../../shared/models/models";


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],   // DatePipe removed — not used in template
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboard implements OnInit {
  stats          = signal<BookingStatisticsDto | null>(null);
  recentBookings = signal<BookingReportDto[]>([]);
  loading        = signal(true);

  kpis = computed(() => {
    const s = this.stats();
    if (!s) return [];
    return [
      { label: 'Total Bookings', value: s.totalBookings.toLocaleString(),      icon: 'bi-ticket-perforated-fill', color: '#0d6efd', bg: 'rgba(13,110,253,.08)',  tag: 'All time' },
      { label: 'Total Revenue',  value: '৳' + s.totalRevenue.toLocaleString(), icon: 'bi-cash-coin',              color: '#10b981', bg: 'rgba(16,185,129,.08)',  tag: 'Revenue'  },
      { label: 'Confirmed',      value: s.confirmedBookings.toLocaleString(),   icon: 'bi-check-circle-fill',      color: '#10b981', bg: 'rgba(16,185,129,.08)',  tag: 'Active'   },
      { label: 'Cancelled',      value: s.cancelledBookings.toLocaleString(),   icon: 'bi-x-circle-fill',          color: '#ef4444', bg: 'rgba(239,68,68,.08)',   tag: 'Refunds'  },
    ];
  });

  statusBreakdown = computed(() => {
    const s = this.stats();
    if (!s || s.totalBookings === 0) return [];
    const pct = (n: number) => Math.round(n / s.totalBookings * 100);
    return [
      { label: 'Confirmed', value: s.confirmedBookings, pct: pct(s.confirmedBookings), bar: 'bg-success' },
      { label: 'Pending',   value: s.pendingBookings,   pct: pct(s.pendingBookings),   bar: 'bg-warning' },
      { label: 'Cancelled', value: s.cancelledBookings, pct: pct(s.cancelledBookings), bar: 'bg-danger'  },
    ];
  });

  topBuses = computed(() => {
    const s = this.stats();
    if (!s) return [];
    const entries = Object.entries(s.bookingsByBus).sort((a, b) => b[1] - a[1]).slice(0, 5);
    const max = entries[0]?.[1] || 1;
    return entries.map(([name, count]) => ({ name, count, pct: Math.round(count / max * 100) }));
  });

  topRoutes = computed(() => {
    const s = this.stats();
    if (!s) return [];
    return Object.entries(s.bookingsByRoute).sort((a, b) => b[1] - a[1]).slice(0, 5)
      .map(([name, count]) => ({ name, count }));
  });

  quickActions = [
    { label: 'Add Bus',      icon: 'bi-bus-front-fill', route: '/admin/buses',      color: '#0d6efd', bg: 'rgba(13,110,253,.07)' },
    { label: 'Add Route',    icon: 'bi-map-fill',        route: '/admin/routes',     color: '#10b981', bg: 'rgba(16,185,129,.07)' },
    { label: 'Add Schedule', icon: 'bi-calendar-plus',   route: '/admin/schedules',  color: '#f59e0b', bg: 'rgba(245,158,11,.07)' },
    { label: 'Passengers',   icon: 'bi-people-fill',     route: '/admin/passengers', color: '#6366f1', bg: 'rgba(99,102,241,.07)' },
  ];

  constructor(private svc: AdminBookingService) {}

  ngOnInit() {
    this.svc.getStatistics().subscribe({
      next: s => { this.stats.set(s); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
    this.svc.getAll({ page: 1, pageSize: 8 }).subscribe({
      next: b => this.recentBookings.set(b)
    });
  }

  /** Safe ISO → "dd MMM yyyy, HH:mm" without DatePipe import */
  formatDate(iso: string): string {
    if (!iso) return '—';
    try {
      const d = new Date(iso);
      return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
             + ', '
             + d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false });
    } catch { return iso; }
  }
}
