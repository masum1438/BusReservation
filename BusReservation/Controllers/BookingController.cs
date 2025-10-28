using BusReservation.Application.DTOs;
using BusReservation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _booking;
        public BookingController(IBookingService booking) => _booking = booking;

        [HttpGet("{busScheduleId}/seatplan")]
        public async Task<IActionResult> GetSeatPlan(Guid busScheduleId)
        {
            var dto = await _booking.GetSeatPlanAsync(busScheduleId);
            return Ok(dto);
        }

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookSeatInputDto input)
        {
            var result = await _booking.BookSeatAsync(input);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
