using BusReservation.Application.DTOs;
using BusReservation.Application.Interfaces;
using BusReservation.Domain.Entities;

namespace BusReservation.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ITicketRepository _ticketRepo;

        public BookingService(ISeatRepository seatRepo, ITicketRepository ticketRepo)
        {
            _seatRepo = seatRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId)
        {
            var seats = await _seatRepo.GetSeatsByScheduleIdAsync(busScheduleId);

            var dto = new SeatPlanDto
            {
                BusScheduleId = busScheduleId,
                Seats = seats.Select(seat => new SeatDto
                {
                    SeatId = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Row = seat.Row,
                    Status = seat.Status
                }).OrderBy(s => s.Row).ThenBy(s => s.SeatNumber).ToList()
            };

            return dto;
        }

        public async Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input)
        {
            var seats = await _seatRepo.GetSeatsByScheduleIdAsync(input.BusScheduleId);
            var seat = seats.FirstOrDefault(s => s.Id == input.SeatId);

            if (seat == null)
                return new BookSeatResultDto { Success = false, Message = "Seat not found" };

            if (seat.Status != SeatStatus.Available)
                return new BookSeatResultDto { Success = false, Message = "Seat is not available" };

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = seat.Id,
                PassengerName = input.PassengerName,
                Mobile = input.Mobile,
                BoardingPoint = input.BoardingPoint,
                DroppingPoint = input.DroppingPoint,
                BookedAt = DateTime.UtcNow,
                IsConfirmed = input.ConfirmNow
            };

            seat.Ticket = ticket;
            seat.Status = input.ConfirmNow ? SeatStatus.Sold : SeatStatus.Booked;

            await _ticketRepo.AddAsync(ticket);
            _seatRepo.Update(seat);
            await _seatRepo.SaveChangesAsync();

            return new BookSeatResultDto { Success = true, Message = "Seat booked successfully", TicketId = ticket.Id };
        }
    }
}
