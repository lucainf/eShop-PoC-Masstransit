using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Login.Models.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Login.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration) : ControllerBase
{
    /* User Actions */
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginForm model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = GetToken(user.Id);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }
    
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("who-am-i")]
    public IActionResult WhoAmI()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
            return Ok(User.Identity.Name);
        
        return Unauthorized();
    }
    
    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterForm model)
    {
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest();
    }
    
    private JwtSecurityToken GetToken(string UserId)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(3),
            claims: new List<Claim>
            {
                new(ClaimTypes.Name, UserId)
            },
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

}