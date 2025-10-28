using BusReservation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchSvc;
        public SearchController(ISearchService searchSvc) => _searchSvc = searchSvc;

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime? journeyDate = null)
        {
            var list = await _searchSvc.SearchAvailableBusesAsync(from, to, journeyDate);
            return Ok(list);
        }
    }
}
