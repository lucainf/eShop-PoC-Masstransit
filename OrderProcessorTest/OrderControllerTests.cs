using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OrderProcessor.Controllers;
using OrderProcessor.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using MassTransit;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;

[TestFixture]
public class OrderControllerTests
{
    private OrderContext _orderContext;
    private Mock<IMapper> _mapperMock;
    private Mock<IRequestClient<GetPriceRequest>> _getPriceClientMock;
    private Mock<IRequestClient<IsAddressIDValidRequest>> _isAddressIDValidClientMock;
    private OrderController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<OrderContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _orderContext = new OrderContext(options);
        _mapperMock = new Mock<IMapper>();
        _getPriceClientMock = new Mock<IRequestClient<GetPriceRequest>>();
        _isAddressIDValidClientMock = new Mock<IRequestClient<IsAddressIDValidRequest>>();

        _controller = new OrderController(
            _orderContext,
            _mapperMock.Object,
            _getPriceClientMock.Object,
            _isAddressIDValidClientMock.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        _orderContext.Database.EnsureDeleted();
        _orderContext.Dispose();
    }

    [Test]
    public void View_OrderDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = _controller.View(1);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void View_OrderExistsAndUserIsOwner_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var order = new Order { Id = 1, UserId = "testuser" };
        _orderContext.Orders.Add(order);
        _orderContext.SaveChanges();

        var orderDto = new OrderProcessor.Models.Dto.Order { Id = 1 };
        _mapperMock.Setup(m => m.Map<OrderProcessor.Models.Dto.Order>(order)).Returns(orderDto);

        // Act
        var result = _controller.View(1) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        Assert.AreEqual(orderDto, result.Value);
    }

    [Test]
    public void View_UserIsNotOwner_ReturnsNotFound()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var order = new Order { Id = 1, UserId = "otheruser" };
        _orderContext.Orders.Add(order);
        _orderContext.SaveChanges();

        // Act
        var result = _controller.View(1);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public void View_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = _controller.View(1);

        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }
}