import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BookingService } from '../../services/booking.service';

@Component({
  selector: 'app-booking',
  imports: [FormsModule,CommonModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class Booking implements OnInit {
  @Input() busScheduleId!: string;
  @Input() selectedSeats!: any[];

  passengers: any[] = [];

  constructor(public activeModal: NgbActiveModal, private bookingService: BookingService) {}

  ngOnInit(): void {
    this.passengers = this.selectedSeats.map(s => ({
      seatId: s.seatId,
      seatNumber: s.seatNumber,
      passengerName: '',
      mobile: '',
      boardingPoint: '',
      droppingPoint: '',
      confirmNow: true
    }));
  }

  confirmBookings(): void {
    const requests = this.passengers.map(p =>
      this.bookingService.bookSeat({
        busScheduleId: this.busScheduleId,
        seatId: p.seatId,
        passengerName: p.passengerName,
        mobile: p.mobile,
        boardingPoint: p.boardingPoint,
        droppingPoint: p.droppingPoint,
        confirmNow: p.confirmNow
      })
    );

    Promise.all(requests.map(r => r.toPromise()))
      .then(() => this.activeModal.close('success'))
      .catch(() => alert('One or more bookings failed.'));
  }
}
