using Microsoft.AspNetCore.Mvc;
using Products.Entities;

namespace Products.Controllers;

[Route("products")]
public class ProductsController(ProductsContext productsContext): ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(productsContext.Products);
    }
    
    [HttpGet]
    [Route("category/{category}")]
    public IActionResult GetProduct(int category)
    {
        if (productsContext.Categories.Find(category) == null)
        {
            return NotFound();
        }
        
        return Ok(productsContext.Products.Where(p => p.CategoryId == category));
    }
}