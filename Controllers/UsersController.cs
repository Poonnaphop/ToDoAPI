using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ToDoAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ToDoAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{

    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Get([FromBody] DTOs.Login data)
    {
        var db = new ToDoDbContext();

        var user = new Models.User();
        var salt = Program.GenerateSalt();
        Console.WriteLine("generate salt");
        string hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password:data.Password,
                salt:Convert.FromBase64String(salt),
                prf:KeyDerivationPrf.HMACSHA1,
                iterationCount:10000,
                numBytesRequested:256/8
            )
        );
        Console.WriteLine("hash pass");
        user.Id = data.Id;
        user.Password = hash;
        user.Salt = salt;

        db.User.Add(user);
        db.SaveChanges();

        var desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(
            new Claim[]{
                new Claim(ClaimTypes.Name,user.Id),
                new Claim(ClaimTypes.Role,"user"),
            }
        );
        desc.NotBefore = DateTime.UtcNow;
        desc.Expires = DateTime.UtcNow.AddHours(3);
        desc.IssuedAt = DateTime.UtcNow;
        desc.Issuer = "ToDoApp";
        desc.Audience = "public";
        desc.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Program.securityKey)
            ),
            SecurityAlgorithms.HmacSha256Signature
        );
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(desc);

        return Ok(new {token = handler.WriteToken(token)}) ;
    }
    
    [HttpPut]
    public IActionResult Put([FromBody] DTOs.Login data)
    {
        var db = new ToDoDbContext();

        var u = db.User.Find(data.Id);
        if(u==null) return NotFound();

        var salt = Program.GenerateSalt();
        Console.WriteLine("generate salt");
        string hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password:data.Password,
                salt:Convert.FromBase64String(salt),
                prf:KeyDerivationPrf.HMACSHA1,
                iterationCount:10000,
                numBytesRequested:256/8
            )
        );

        Console.WriteLine("hash pass");
        u.Password = hash;
        u.Salt = salt;

        db.User.Update(u);
        db.SaveChanges();

        var desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(
            new Claim[]{
                new Claim(ClaimTypes.Name,u.Id),
                new Claim(ClaimTypes.Role,"user"),
            }
        );
        desc.NotBefore = DateTime.UtcNow;
        desc.Expires = DateTime.UtcNow.AddHours(3);
        desc.IssuedAt = DateTime.UtcNow;
        desc.Issuer = "ToDoApp";
        desc.Audience = "public";
        desc.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Program.securityKey)
            ),
            SecurityAlgorithms.HmacSha256Signature
        );
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(desc);

        return Ok(new {token = handler.WriteToken(token)}) ;
    }

}

