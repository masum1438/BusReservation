import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Seat } from '../../models/seat.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../booking/booking';

@Component({
  selector: 'app-seat-plan',
  imports: [FormsModule,CommonModule],
  templateUrl: './seat-plan.html',
  styleUrl: './seat-plan.css'
})
export class SeatPlan implements OnInit {
  busScheduleId = '';
  seats: Seat[] = [];
  loading = false;
  message = '';

  constructor(
    private route: ActivatedRoute,
    private bookingService: BookingService,
    private modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.busScheduleId = this.route.snapshot.paramMap.get('busScheduleId')!;
    this.loadSeats();
  }

  loadSeats(): void {
    this.loading = true;
    this.bookingService.getSeatPlan(this.busScheduleId).subscribe({
      next: (res: any) => {
        this.seats = res.seats.map((s: any) => ({ ...s, selected: false }));
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 0) return;
    seat.selected = !seat.selected;
  }

  openBookingModal(): void {
    const selectedSeats = this.seats.filter(s => s.selected);
    if (!selectedSeats.length) {
      this.message = 'Please select at least one available seat.';
      return;
    }

    const modalRef = this.modalService.open(Booking, { size: 'lg' });
    modalRef.componentInstance.busScheduleId = this.busScheduleId;
    modalRef.componentInstance.selectedSeats = selectedSeats;

    modalRef.result.then(res => {
      if (res === 'success') {
        this.message = 'Booking successful!';
        this.loadSeats();
      }
    });
  }
}