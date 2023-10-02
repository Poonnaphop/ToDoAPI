using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ToDoAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly ILogger<ActivitiesController> _logger;

    public ActivitiesController(ILogger<ActivitiesController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    [Authorize(Roles = "user")]
    public IActionResult Get()
    {
        var db = new ToDoDbContext();

        var activities = from a in db.Activity
        where a.Uid == User.Identity.Name
        select new {
            id = a.Id,
            time = Program.Thai(a.When),
            name = a.Name
        };

        if(!activities.Any()) return NoContent();

        return Ok(activities);
    }

    [HttpGet]
    [Route("{Id}")]
    [Authorize(Roles = "user")]
    public IActionResult Get(uint Id)
    {
        var db = new ToDoDbContext();

        /*
        var activities = from a in db.Activity
        where a.Id == Id && a.Uid == User.Identity.Name
        select new {
            id = a.Id,
            time = Program.Thai(a.When),
            name = a.Name
        };
        if(!activities.Any()) return NotFound();
        */


        var a = db.Activity.Find(Id);
        if(a==null) return NotFound();
        if(a.Uid !=User.Identity.Name) return Unauthorized();
        var Activity = new {
            Id=a.Id,
            name=a.Name,
            when=a.When,
        };

        return Ok(Activity);
    }

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult Post([FromBody] DTOs.Activity data)
    {
        var db = new ToDoDbContext();

        var a = new Models.Activity();
        a.Name = data.Name;
        a.When = data.When;
        a.Uid = User.Identity.Name;

        db.Activity.Add(a);
        db.SaveChanges();

        return Ok(new {id = a.Id});
    }

    [HttpPut]
    [Route("{Id}")]
    [Authorize(Roles = "user")]
    public IActionResult Put(uint Id,[FromBody] DTOs.Activity data)
    {
        var db = new ToDoDbContext();

        var a = db.Activity.Find(Id);
        if(a==null) return NotFound();

        if(a.Uid != User.Identity.Name) return Unauthorized();

        a.Name = data.Name;
        a.When = data.When;

        db.SaveChanges();

        return Ok();
    }

    [HttpDelete]
    [Route("{Id}")]
    [Authorize(Roles = "user")]
    public IActionResult Delete(uint Id)
    {
        var db = new ToDoDbContext();

        var a = db.Activity.Find(Id);
        if(a==null) return NotFound();

        if(a.Uid != User.Identity.Name) return Unauthorized();

        db.Activity.Remove(a);
        db.SaveChanges();
        return Ok();
    }
/*
    [HttpDelete]
    [Authorize(Roles = "user")]
    public IActionResult Delete([FromBody] DTOs.DeleteBody data)
    {
        var db = new ToDoDbContext();

        if(data.Id == null) BadRequest(new {reason="No id"});
        var a = db.Activity.Find(data.Id);
        if(a==null) return NotFound();

        if(a.Uid != User.Identity.Name) return Unauthorized();

        db.Activity.Remove(a);
        db.SaveChanges();
        return Ok();
    }
    */

}

