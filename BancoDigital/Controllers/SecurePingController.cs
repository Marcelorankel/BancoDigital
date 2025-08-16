using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SecurePingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "pong (you are authenticated)" });
}