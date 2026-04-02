using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class SearchController : BaseApiController
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchBuses(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] DateTime journeyDate)
    {
        try
        {
            var result = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}