using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SeatController : ControllerBase
    {
        private readonly ISeatLockService _seatLockService;
        private readonly IBookingService _bookingService;

        public SeatController(
            ISeatLockService seatLockService,
            IBookingService bookingService)
        {
            _seatLockService = seatLockService;
            _bookingService = bookingService;
        }

        [HttpGet("plan/{scheduleId}")]
        public async Task<IActionResult> GetSeatPlan(Guid scheduleId)
        {
            try
            {
                var result = await _bookingService.GetSeatPlanAsync(scheduleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        //[HttpPost("lock")]
        //public async Task<IActionResult> LockSeats([FromBody] SeatLockRequestDto request)
        //{
        //    try
        //    {
        //        // Get user ID from JWT token (simplified for now)
        //        var userId = Guid.NewGuid(); // Replace with actual user ID from claims
        //        var result = await _seatLockService.LockSeatsAsync(request, userId);

        //        if (result.IsSuccess)
        //            return Ok(result);
        //        else
        //            return BadRequest(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}

        //[HttpPost("release")]
        //public async Task<IActionResult> ReleaseSeats([FromBody] SeatReleaseRequestDto request)
        //{
        //    try
        //    {
        //        var result = await _seatLockService.ReleaseSeatsAsync(request);
        //        return Ok(new { success = result, message = "Seats released successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}
    }
}