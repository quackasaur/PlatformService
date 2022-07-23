using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
  [ApiController]
  [Route("api/c/[controller]")]
  public class PlatformsController : ControllerBase
  {
    public PlatformsController()
    {

    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
      return Ok("Inbound connection works");
    }
  }
}
