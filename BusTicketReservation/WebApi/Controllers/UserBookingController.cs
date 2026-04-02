using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/user/bookings")]
[Authorize]
public class UserBookingController : BaseApiController
{
    private readonly IUserBookingService _userBookingService;

    public UserBookingController(IUserBookingService userBookingService)
    {
        _userBookingService = userBookingService;
    }

    /// <summary>Get all bookings for the currently authenticated user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyBookings()
    {
        try
        {
            // FIX: CurrentUserId from JWT claim — was Guid.NewGuid() placeholder
            var result = await _userBookingService.GetUserBookingsAsync(CurrentUserId);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Get a specific ticket by GUID — ownership enforced.</summary>
    [HttpGet("{ticketId:guid}")]
    public async Task<IActionResult> GetTicketById(Guid ticketId)
    {
        try
        {
            var result = await _userBookingService.GetTicketByGuidAsync(ticketId, CurrentUserId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Get a ticket by its human-readable ticket number — ownership enforced.</summary>
    [HttpGet("number/{ticketNumber}")]
    public async Task<IActionResult> GetTicketByNumber(string ticketNumber)
    {
        try
        {
            var result = await _userBookingService.GetTicketByNumberAsync(ticketNumber, CurrentUserId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Cancel a booking — only the owner can cancel.</summary>
    [HttpPost("cancel/{ticketId:guid}")]
    public async Task<IActionResult> CancelBooking(Guid ticketId)
    {
        try
        {
            // FIX: CurrentUserId ensures only the ticket owner can cancel
            var result = await _userBookingService.CancelUserBookingAsync(ticketId, CurrentUserId);
            return Ok(new { success = result, message = "Booking cancelled successfully" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}