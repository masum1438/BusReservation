using Application.DTOs.Admin;
using Application.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin;

// ══════════════════════════════════════════════════════════════════════════════
// BUS CONTROLLER
// FIX: [Authorize(Roles = "Admin")] — was just [Authorize], any user could access
// ══════════════════════════════════════════════════════════════════════════════

[Route("api/admin/buses")]
[Authorize(Roles = "Admin")]
public class BusController : BaseApiController
{
    private readonly IAdminBusService _busService;
    public BusController(IAdminBusService busService) => _busService = busService;

    [HttpPost]
    public async Task<IActionResult> CreateBus([FromBody] BusCreateDto dto)
    {
        try { return Ok(await _busService.CreateBusAsync(dto)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBus(Guid id, [FromBody] BusCreateDto dto)
    {
        try { return Ok(await _busService.UpdateBusAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBus(Guid id)
    {
        try { await _busService.DeleteBusAsync(id); return Ok(new { message = "Bus deleted" }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBus(Guid id)
    {
        try { return Ok(await _busService.GetBusAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBuses()
    {
        try { return Ok(await _busService.GetAllBusesAsync()); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// ROUTE CONTROLLER
// FIX: [Authorize(Roles = "Admin")]
// ══════════════════════════════════════════════════════════════════════════════

[Route("api/admin/routes")]
[Authorize(Roles = "Admin")]
public class RouteController : BaseApiController
{
    private readonly IAdminRouteService _routeService;
    public RouteController(IAdminRouteService routeService) => _routeService = routeService;

    [HttpPost]
    public async Task<IActionResult> CreateRoute([FromBody] RouteCreateDto dto)
    {
        try { return Ok(await _routeService.CreateRouteAsync(dto)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] RouteCreateDto dto)
    {
        try { return Ok(await _routeService.UpdateRouteAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        try { await _routeService.DeleteRouteAsync(id); return Ok(new { message = "Route deleted" }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoute(Guid id)
    {
        try { return Ok(await _routeService.GetRouteAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        try { return Ok(await _routeService.GetAllRoutesAsync()); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// SCHEDULE CONTROLLER
// FIX: [Authorize(Roles = "Admin")]
// ══════════════════════════════════════════════════════════════════════════════

[Route("api/admin/schedules")]
[Authorize(Roles = "Admin")]
public class ScheduleController : BaseApiController
{
    private readonly IAdminScheduleService _scheduleService;
    public ScheduleController(IAdminScheduleService scheduleService) => _scheduleService = scheduleService;

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleCreateDto dto)
    {
        try { return Ok(await _scheduleService.CreateScheduleAsync(dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] ScheduleCreateDto dto)
    {
        try { return Ok(await _scheduleService.UpdateScheduleAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        try { await _scheduleService.DeleteScheduleAsync(id); return Ok(new { message = "Schedule deleted" }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        try { return Ok(await _scheduleService.GetScheduleAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSchedules()
    {
        try { return Ok(await _scheduleService.GetAllSchedulesAsync()); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("bus/{busId:guid}")]
    public async Task<IActionResult> GetByBus(Guid busId)
    {
        try { return Ok(await _scheduleService.GetSchedulesByBusAsync(busId)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("route/{routeId:guid}")]
    public async Task<IActionResult> GetByRoute(Guid routeId)
    {
        try { return Ok(await _scheduleService.GetSchedulesByRouteAsync(routeId)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// BOOKING MANAGEMENT CONTROLLER
// FIX: [Authorize(Roles = "Admin")]
// ══════════════════════════════════════════════════════════════════════════════

[Route("api/admin/bookings")]
[Authorize(Roles = "Admin")]
public class BookingManagementController : BaseApiController
{
    private readonly IAdminBookingService _bookingService;
    public BookingManagementController(IAdminBookingService bookingService) => _bookingService = bookingService;

    [HttpGet]
    public async Task<IActionResult> GetAllBookings([FromQuery] BookingFilterDto filter)
    {
        try { return Ok(await _bookingService.GetAllBookingsAsync(filter)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{ticketId:guid}")]
    public async Task<IActionResult> GetBookingDetails(Guid ticketId)
    {
        try { return Ok(await _bookingService.GetBookingDetailsAsync(ticketId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("by-number/{ticketNumber}")]
    public async Task<IActionResult> GetByTicketNumber(string ticketNumber)
    {
        try { return Ok(await _bookingService.GetBookingByTicketNumberAsync(ticketNumber)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{ticketId:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid ticketId, [FromBody] string reason)
    {
        try
        {
            await _bookingService.CancelBookingAsync(ticketId, reason);
            return Ok(new { message = "Booking cancelled successfully" });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        try { return Ok(await _bookingService.GetBookingStatisticsAsync(from, to)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReport([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        try { return Ok(await _bookingService.GetBookingsByDateRangeAsync(from, to)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}