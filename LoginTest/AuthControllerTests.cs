using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Login.Controllers;
using Login.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<IConfiguration> _configurationMock;
    private AuthController _controller;

    [SetUp]
    public void SetUp()
    {
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("46dc1c7200009ba12c735fa7643f3da89c0161f6db8a3256079a20ab7c85fb76");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("http://localhost:5000");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("http://localhost:5000");

        _controller = new AuthController(_userManagerMock.Object, _configurationMock.Object);
    }

    [Test]
    public async Task Login_ValidUser_ReturnsToken()
    {
        // Arrange
        var loginForm = new LoginForm { Email = "test@example.com", Password = "Password123" };
        var user = new IdentityUser { Id = "1", Email = "test@example.com" };

        _userManagerMock.Setup(um => um.FindByEmailAsync(loginForm.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginForm.Password)).ReturnsAsync(true);

        // Act
        var result = await _controller.Login(loginForm) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var token = result.Value.GetType().GetProperty("token").GetValue(result.Value, null);
        Assert.IsNotNull(token);
    }

    [Test]
    public async Task Login_InvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginForm = new LoginForm { Email = "test@example.com", Password = "WrongPassword" };
        var user = new IdentityUser { Id = "1", Email = "test@example.com" };

        _userManagerMock.Setup(um => um.FindByEmailAsync(loginForm.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginForm.Password)).ReturnsAsync(false);

        // Act
        var result = await _controller.Login(loginForm);

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public void WhoAmI_AuthenticatedUser_ReturnsUserName()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = _controller.WhoAmI() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("testuser", result.Value);
    }

    [Test]
    public void WhoAmI_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = _controller.WhoAmI();

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public async Task Register_ValidUser_ReturnsOk()
    {
        // Arrange
        var registerForm = new RegisterForm { Email = "test@example.com", Password = "Password123" };
        var user = new IdentityUser { UserName = registerForm.Email, Email = registerForm.Email };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerForm.Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(registerForm);

        // Assert
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task Register_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var registerForm = new RegisterForm { Email = "test@example.com", Password = "Password123" };
        var user = new IdentityUser { UserName = registerForm.Email, Email = registerForm.Email };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerForm.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await _controller.Register(registerForm);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }
}