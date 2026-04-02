using Application.DTOs;
using Application.Services;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/seats")]
[Authorize]
public class SeatLockController : BaseApiController
{
    private readonly ISeatLockService _seatLockService;
    private readonly ISeatAvailabilityService _seatAvailabilityService;
    private readonly IBookingService _bookingService;

    public SeatLockController(
        ISeatLockService seatLockService,
        ISeatAvailabilityService seatAvailabilityService,
        IBookingService bookingService)
    {
        _seatLockService = seatLockService;
        _seatAvailabilityService = seatAvailabilityService;
        _bookingService = bookingService;
    }

    [HttpGet("plan/{scheduleId:guid}")]
    public async Task<IActionResult> GetSeatPlan(Guid scheduleId)
    {
        try
        {
            var result = await _bookingService.GetSeatPlanAsync(scheduleId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("lock")]
    public async Task<IActionResult> LockSeats([FromBody] SeatLockRequestDto request)
    {
        try
        {
            // FIX: CurrentUserId from JWT — was Guid.NewGuid() placeholder
            var result = await _seatLockService.LockSeatsAsync(request, CurrentUserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("release")]
    public async Task<IActionResult> ReleaseSeats([FromBody] SeatReleaseRequestDto request)
    {
        try
        {
            // FIX: CurrentUserId passed so only owner can release their own locks
            var result = await _seatLockService.ReleaseSeatsAsync(request, CurrentUserId);
            return Ok(new { success = result, message = "Seats released successfully" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("check/{seatId:guid}")]
    public async Task<IActionResult> CheckSeatAvailability(Guid seatId)
    {
        try
        {
            var isAvailable = await _seatAvailabilityService.IsSeatAvailableAsync(seatId);
            return Ok(new { seatId, isAvailable });
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("check-bulk")]
    public async Task<IActionResult> CheckMultipleSeatsAvailability([FromBody] SeatAvailabilityCheckDto request)
    {
        try
        {
            var availability = await _seatAvailabilityService.CheckSeatsAvailabilityAsync(
                request.BusScheduleId, request.SeatNumbers);
            return Ok(availability);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}