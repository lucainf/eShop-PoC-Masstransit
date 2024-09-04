using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using UserAddresses.Entities;

namespace UserAddresses.Controllers;

[Route("addresses")]
public class AddressController(AddressesContext addressesContext, IMapper mapper): ControllerBase
{
    [HttpGet]
    public IActionResult GetAddresses()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null)
        {
            return Unauthorized();
        }
        
        return Ok(
            addressesContext.Addresses.Where(a => a.UserId == username && !a.Deleted)
                .ProjectTo<Models.Dto.Address>(mapper.ConfigurationProvider)
            );
    }
    
    [HttpPost]
    public IActionResult AddAddress([FromBody] Models.Request.Address address)
    {
        // map Request.Address to Entity.Address using automapper
        var addressEntity = mapper.Map<Entities.Address>(address);
        
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null)
        {
            return Unauthorized();
        }
        
        addressEntity.UserId = username;
        addressesContext.Addresses.Add(addressEntity);
        addressesContext.SaveChanges();
        
        return Ok(addressEntity.Id);
    }
    
    [HttpDelete]
    public IActionResult DeleteAddress([FromBody] int id)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null)
        {
            return Unauthorized();
        }
        
        var address = addressesContext.Addresses.Find(id);
        if (address == null || address.UserId != username)
        {
            return NotFound();
        }
        
        address.Deleted = true;
        addressesContext.SaveChanges();
        
        return Ok();
    }
}