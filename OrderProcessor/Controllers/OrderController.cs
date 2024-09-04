using System.Security.Claims;
using System.Xml;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessor.Entities;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;

namespace OrderProcessor.Controllers;

[ApiController]
[Route("orders")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class OrderController(
    OrderContext orderContext,
    IMapper mapper,
    IRequestClient<GetPriceRequest> getPriceClient,
    IRequestClient<IsAddressIDValidRequest> isAddressIDValidClient
    ): ControllerBase
{
    [HttpGet]
    [Route("view/{orderId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult View(int orderId)
    {
        // Step 1: Get the user ID from the token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
    
        // Step 2: Get the order from the database
        var order = orderContext.Orders
            .Find(orderId);
    
        if (order == null)
        {
            return NotFound();
        }
    
        // Step 3: Check if the user is the owner of the order
        if (order.UserId != userId)
        {
            return Forbid(); // 403 Forbidden
        }
    
        // Step 4: Return the order
        return Ok(mapper.Map<Models.Dto.Order>(order));
    }
    
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Create([FromBody] Models.Request.Order order)
    {
        // Step 1: Get the user ID from the token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        // Step 2: Create the order
        var orderEntity = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            ProductIds = order.Products,
        };
        
        // Step 3: Calculate the total price
        decimal totalPrice = 0;
        foreach (var productId in order.Products)
        {
            var request = new GetPriceRequest { Id = productId };
            var response = getPriceClient.GetResponse<GetPriceResponse>(request).Result;
            totalPrice += response.Message.Price;
        }
        
        orderEntity.Total = totalPrice;
        
        // Step 4: Check if the address id entry exists and is owned by the user
        var requestAddress = new IsAddressIDValidRequest { AddressID = order.AddressId, UserId = userId };
        var responseAddress = isAddressIDValidClient.GetResponse<IsAddressIDValidResponse>(requestAddress).Result;
        
        if (!responseAddress.Message.Success)
        {
            return BadRequest("Invalid address ID");
        }
        
        // Step 5: Save the order to the database
        orderContext.Orders.Add(orderEntity);
        orderContext.SaveChanges();

        // Step 6: Return the order ID
        return Ok(new { orderId = orderEntity.Id });
    }
}