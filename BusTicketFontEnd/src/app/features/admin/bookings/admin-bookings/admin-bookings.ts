import { CommonModule } from "@angular/common";
import { Component, OnInit, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AdminBookingService } from "../../../../core/services/api.services";
import { BookingReportDto, BookingFilterDto } from "../../../../shared/models/models";


@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-bookings.html',
  styleUrl: './admin-bookings.scss'
})
export class AdminBookings implements OnInit {
  bookings      = signal<BookingReportDto[]>([]);
  loading       = signal(true);
  cancelling    = signal<string | null>(null);
  cancelTarget  = signal<BookingReportDto | null>(null);
  cancelReason  = '';
  total         = signal(0);

  filter: BookingFilterDto = { page: 1, pageSize: 20 };

  summaryCards() {
    const list = this.bookings();
    const confirmed = list.filter(b => b.status === 'Confirmed').length;
    const cancelled = list.filter(b => b.status === 'Cancelled').length;
    const revenue   = list.filter(b => b.status === 'Confirmed').reduce((s, b) => s + b.price, 0);
    return [
      { label: 'Confirmed',      value: confirmed,          color: '#10b981' },
      { label: 'Cancelled',      value: cancelled,          color: '#ef4444' },
      { label: 'Page Revenue',   value: '৳' + revenue.toLocaleString(), color: '#0d6efd' },
      { label: 'On This Page',   value: list.length,        color: '#64748b' },
    ];
  }

  constructor(private svc: AdminBookingService) {}
  
  ngOnInit() { 
    this.search(); 
  }

  search() {
    this.loading.set(true);
    
    // Prepare filter with proper date formatting for API
    const searchFilter: BookingFilterDto = {
      page: this.filter.page,
      pageSize: this.filter.pageSize,
      ticketNumber: this.filter.ticketNumber || undefined,
      busName: this.filter.busName || undefined,
      status: this.filter.status || undefined
    };
    
    // Add dates if they exist (convert to ISO format for API)
    if (this.filter.fromDate) {
      searchFilter.fromDate = this.convertToUTC(this.filter.fromDate);
    }
    if (this.filter.toDate) {
      searchFilter.toDate = this.convertToUTC(this.filter.toDate);
    }
    
    this.svc.getAll(searchFilter).subscribe({
      next: b => { 
        this.bookings.set(b); 
        this.total.set(b.length + (this.filter.page - 1) * this.filter.pageSize); 
        this.loading.set(false); 
      },
      error: (err) => {
        console.error('Failed to load bookings:', err);
        this.loading.set(false);
        this.bookings.set([]);
      }
    });
  }

  reset() { 
    this.filter = { page: 1, pageSize: 20 }; 
    this.cancelReason = ''; 
    this.search(); 
  }
  
  prevPage() { 
    if (this.filter.page > 1) { 
      this.filter.page--; 
      this.search(); 
    } 
  }
  
  nextPage() { 
    this.filter.page++; 
    this.search(); 
  }

  cancel(b: BookingReportDto) { 
    this.cancelReason = ''; 
    this.cancelTarget.set(b); 
  }

  confirmCancel() {
    const t = this.cancelTarget();
    if (!t) return;
    this.cancelling.set(t.ticketId);
    this.svc.cancel(t.ticketId, this.cancelReason || 'Cancelled by admin').subscribe({
      next: () => {
        this.cancelling.set(null);
        this.cancelTarget.set(null);
        this.bookings.update(list => list.map(x => x.ticketId === t.ticketId ? { ...x, status: 'Cancelled' } : x));
      },
      error: (err) => {
        console.error('Failed to cancel booking:', err);
        this.cancelling.set(null);
      }
    });
  }

  // Date formatting methods (UTC-based to avoid timezone shifts)
  private formatDate(isoString: string): string {
    if (!isoString) return '—';
    try {
      const date = new Date(isoString);
      if (isNaN(date.getTime())) return '—';
      return date.toLocaleDateString('en-GB', { 
        day: '2-digit', 
        month: 'short', 
        year: 'numeric',
        timeZone: 'UTC'
      });
    } catch {
      return '—';
    }
  }

  formatDateShort(isoString: string): string {
    if (!isoString) return '—';
    try {
      const date = new Date(isoString);
      if (isNaN(date.getTime())) return '—';
      return date.toLocaleDateString('en-GB', { 
        day: '2-digit', 
        month: 'short',
        timeZone: 'UTC'
      });
    } catch {
      return '—';
    }
  }

  formatTime(isoString: string): string {
    if (!isoString) return '—';
    try {
      const date = new Date(isoString);
      if (isNaN(date.getTime())) return '—';
      return date.toLocaleTimeString('en-GB', { 
        hour: '2-digit', 
        minute: '2-digit',
        hour12: false,
        timeZone: 'UTC'
      });
    } catch {
      return '—';
    }
  }

  formatDateTime(isoString: string): string {
    if (!isoString) return '—';
    try {
      const date = new Date(isoString);
      if (isNaN(date.getTime())) return '—';
      const dateStr = date.toLocaleDateString('en-GB', { 
        day: '2-digit', 
        month: 'short', 
        year: 'numeric',
        timeZone: 'UTC'
      });
      const timeStr = date.toLocaleTimeString('en-GB', { 
        hour: '2-digit', 
        minute: '2-digit',
        hour12: false,
        timeZone: 'UTC'
      });
      return `${dateStr}, ${timeStr}`;
    } catch {
      return '—';
    }
  }

  private convertToUTC(dateString: string): string {
    if (!dateString) return '';
    try {
      // Create date from YYYY-MM-DD and set to start of day in UTC
      const date = new Date(dateString + 'T00:00:00Z');
      if (isNaN(date.getTime())) return '';
      return date.toISOString();
    } catch {
      return '';
    }
  }
}