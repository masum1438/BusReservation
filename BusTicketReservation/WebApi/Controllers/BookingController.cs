using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class BookingController : BaseApiController
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("seat-plan/{busScheduleId:guid}")]
    public async Task<IActionResult> GetSeatPlan(Guid busScheduleId)
    {
        try
        {
            var result = await _bookingService.GetSeatPlanAsync(busScheduleId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookSeat([FromBody] BookSeatInputDto input)
    {
        try
        {
            // FIX: Use CurrentUserId from JWT — was Guid.NewGuid() placeholder
            var result = await _bookingService.BookSeatAsync(input, CurrentUserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("book-multiple")]
    public async Task<IActionResult> BookMultipleSeats([FromBody] MultipleBookSeatInputDto input)
    {
        try
        {
            var result = await _bookingService.BookMultipleSeatsAsync(input, CurrentUserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("ticket/{ticketId:guid}")]
    public async Task<IActionResult> GetTicketById(Guid ticketId)
    {
        try
        {
            var result = await _bookingService.GetTicketByGuidAsync(ticketId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("ticket/number/{ticketNumber}")]
    public async Task<IActionResult> GetTicketByNumber(string ticketNumber)
    {
        try
        {
            var result = await _bookingService.GetTicketByNumberAsync(ticketNumber);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}