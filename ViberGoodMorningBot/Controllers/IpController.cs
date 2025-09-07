using Microsoft.AspNetCore.Mvc;

namespace ViberGoodMorningBot.Controllers;

[ApiController]
[Route("showip")]
public class IpController : ControllerBase
{
    [HttpGet]
    public IActionResult GetIp([FromQuery] string uid)
    {
        var realIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        return Ok(new { userId = uid, realIp });
    }
}
