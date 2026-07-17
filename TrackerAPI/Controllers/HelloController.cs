using Microsoft.AspNetCore.Mvc;


namespace TrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok (new { Message = "Hellow World!"});
    }
}