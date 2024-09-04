using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AddressesTest;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using UserAddresses.Controllers;
using UserAddresses.Entities;
using UserAddresses.Models.Dto;
using UserAddresses.Models.Request;

[TestFixture]
public class AddressControllerTests
{
    private Mock<AddressesContext> _addressesContextMock;
    private Mock<IMapper> _mapperMock;
    private AddressController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AddressesContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _addressesContextMock = new Mock<AddressesContext>(options);
        _mapperMock = new Mock<IMapper>();

        _controller = new AddressController(_addressesContextMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetAddresses_AuthenticatedUser_ReturnsAddresses()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var addresses = new List<UserAddresses.Entities.Address>
        {
            new UserAddresses.Entities.Address { Id = 1, UserId = "testuser", Deleted = false },
            new UserAddresses.Entities.Address { Id = 2, UserId = "testuser", Deleted = false }
        };

        var mockDbSet = MockDbSet.GetQueryableMockDbSet(addresses);
        _addressesContextMock.Setup(c => c.Addresses).Returns(mockDbSet);

        var addressDtos = new List<UserAddresses.Models.Dto.Address>
        {
            new UserAddresses.Models.Dto.Address { Id = 1 },
            new UserAddresses.Models.Dto.Address { Id = 2 }
        };

        _mapperMock.Setup(m => m.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.CreateMap<UserAddresses.Entities.Address, UserAddresses.Models.Dto.Address>()));
        _mapperMock.Setup(m => m.ProjectTo<UserAddresses.Models.Dto.Address>(It.IsAny<IQueryable<UserAddresses.Entities.Address>>(), null)).Returns(addressDtos.AsQueryable());

        // Act
        var result = _controller.GetAddresses() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        foreach (var address in (result.Value as IEnumerable<UserAddresses.Models.Dto.Address>).ToList())
        {
            Assert.Contains(address.Id, addressDtos.Select(a => a.Id).ToList());
        }
    }

    [Test]
    public void GetAddresses_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = _controller.GetAddresses();

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public void AddAddress_AuthenticatedUser_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var addressRequest = new UserAddresses.Models.Request.Address { Street = "123 Main St" };
        var addressEntity = new UserAddresses.Entities.Address { Id = 1, UserId = "testuser", Street = "123 Main St" };

        _mapperMock.Setup(m => m.Map<UserAddresses.Entities.Address>(addressRequest)).Returns(addressEntity);
        _addressesContextMock.Setup(c => c.Addresses.Add(addressEntity));
        _addressesContextMock.Setup(c => c.SaveChanges()).Returns(1);

        // Act
        var result = _controller.AddAddress(addressRequest) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        Assert.AreEqual(addressEntity.Id, result.Value);
    }

    [Test]
    public void AddAddress_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var addressRequest = new UserAddresses.Models.Request.Address { Street = "123 Main St" };

        // Act
        var result = _controller.AddAddress(addressRequest);

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public void DeleteAddress_AuthenticatedUser_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var address = new UserAddresses.Entities.Address { Id = 1, UserId = "testuser", Deleted = false };

        _addressesContextMock.Setup(c => c.Addresses.Find(1)).Returns(address);
        _addressesContextMock.Setup(c => c.SaveChanges()).Returns(1);

        // Act
        var result = _controller.DeleteAddress(1) as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public void DeleteAddress_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = _controller.DeleteAddress(1);

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public void DeleteAddress_AddressNotFound_ReturnsNotFound()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _addressesContextMock.Setup(c => c.Addresses.Find(1)).Returns((UserAddresses.Entities.Address)null);

        // Act
        var result = _controller.DeleteAddress(1);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}