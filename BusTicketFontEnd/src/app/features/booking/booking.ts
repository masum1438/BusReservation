// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-booking',
//   imports: [],
//   templateUrl: './booking.html',
//   styleUrl: './booking.scss',
// })
// export class Booking {}

import { Component, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../core/services/api.services';
import { SeatPlanDto, SeatDto, BookSeatInputDto } from '../../shared/models/models';
import { DateUtils } from '../../core/utils/date-utils';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './booking.html',
  styleUrl: './booking.scss'
   
  
})
export class Booking implements OnInit {
  plan         = signal<SeatPlanDto | null>(null);
  selectedSeat = signal<SeatDto | null>(null);
  pageLoading  = signal(true);
  loading      = signal(false);
  error        = signal('');
  success      = signal('');
  scheduleId   = '';
  fmt          = DateUtils;

  form: Partial<BookSeatInputDto> = {
    passengerName: '', mobileNumber: '', email: '',
    boardingPoint: '', droppingPoint: ''
  };

  constructor(private route: ActivatedRoute, private router: Router, private svc: BookingService) {}

  ngOnInit() {
    this.scheduleId = this.route.snapshot.paramMap.get('scheduleId') ?? '';
    this.svc.getSeatPlan(this.scheduleId).subscribe({
      next: p  => { this.plan.set(p); this.form.busScheduleId = this.scheduleId; this.pageLoading.set(false); },
      error: () => this.pageLoading.set(false)
    });
  }

  selectSeat(seat: SeatDto) {
    if (!seat.isAvailable) return;
    this.selectedSeat.set(seat);
    this.form.seatId = seat.seatId;
  }

  book() {
    if (!this.selectedSeat()) return;
    this.loading.set(true); this.error.set(''); this.success.set('');
    this.svc.bookSeat(this.form as BookSeatInputDto).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.isSuccess) {
          this.success.set(`Booking confirmed! Ticket: ${res.ticketNumber}`);
          setTimeout(() => this.router.navigate(['/my-bookings']), 2000);
        } else {
          this.error.set(res.message || 'Booking failed');
        }
      },
      error: () => { this.loading.set(false); this.error.set('Booking failed. Please try again.'); }
    });
  }
}
