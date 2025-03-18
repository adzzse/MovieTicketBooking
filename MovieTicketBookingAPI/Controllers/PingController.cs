using Microsoft.AspNetCore.Mvc;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/pings")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet("server")]
        public IActionResult GetPingInfo() => Ok(new {
            message = "PING"
        });
    }
}
