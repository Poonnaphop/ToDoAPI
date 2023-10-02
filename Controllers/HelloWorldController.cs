using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ToDoAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<HelloWorldController> _logger;

    public HelloWorldController(ILogger<HelloWorldController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return StatusCode(202, new {mesg ="Hello world"});
    }

    [HttpGet]
    [Route("testdb")]
    [Authorize(Roles = "user")]
    public IActionResult Get2()
    {
        return Ok(new {
            Id = User.Identity.Name
        });
    }
}

