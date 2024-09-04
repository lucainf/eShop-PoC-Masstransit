using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;

namespace Backoffice.Controllers;

[ApiController]
[Route("admin")]
public class AdminController(
        IRequestClient<DeleteUserRequest> deleteUserClient,
        IRequestClient<DeleteProductRequest> deleteProductClient,
        IRequestClient<AddProductRequest> addProductClient,
        IRequestClient<UpdateProductRequest> updateProductClient,
        IRequestClient<AddCategoryRequest> addCategoryClient,
        IRequestClient<UpdateCategoryRequest> updateCategoryClient,
        IRequestClient<DeleteCategoryRequest> deleteCategoryClient
    ) : ControllerBase
{

    [HttpDelete]
    [Route("user/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var request = new DeleteUserRequest { UserId = id };
        var response = await deleteUserClient.GetResponse<DeleteUserResponse>(request);
        
        if (!response.Message.Success)
        {
            return NotFound($"User with id {id} not found");
        }
        
        return Ok($"User with id {id} has been deleted");
    }
    
    [HttpDelete]
    [Route("product/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var request = new DeleteProductRequest { ProductId = id };
        var response = await deleteProductClient.GetResponse<DeleteProductResponse>(request);
        
        if (!response.Message.Success)
        {
            return NotFound($"Product with id {id} not found");
        }
        
        return Ok($"Product with id {id} has been deleted");
    }
    
    [HttpPost]
    [Route("product")]
    public async Task<IActionResult> AddProduct([FromBody] Models.Request.Product product)
    {
        // check if every field is not null or empty
        if (string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Description) || product.Price == null || product.CategoryId == null)
        {
            return BadRequest("All fields are required");
        }
        
        var request = new AddProductRequest
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Value,
            CategoryId = product.CategoryId.Value
        };
        
        var response = await addProductClient.GetResponse<AddProductResponse>(request);
        
        return Ok($"Product with id {response.Message.ProductId} has been added");
    }
    
    [HttpPut]
    [Route("product/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Models.Request.Product product)
    {
        var request = new UpdateProductRequest
        {
            ProductId = id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId
        };
        
        var response = await updateProductClient.GetResponse<UpdateProductResponse>(request);
        
        if (!response.Message.Success)
        {
            return NotFound($"Product with id {id} not found");
        }
        
        return Ok($"Product with id {id} has been updated");
    }
    
    [HttpPost]
    [Route("category")]
    public async Task<IActionResult> AddCategory([FromBody] Models.Request.Category category)
    {
        // check if every field is not null or empty
        if (string.IsNullOrEmpty(category.Name) || string.IsNullOrEmpty(category.Description))
        {
            return BadRequest("All fields are required");
        }
        
        var request = new AddCategoryRequest
        {
            Name = category.Name,
            Description = category.Description
        };
        
        var response = await addCategoryClient.GetResponse<AddCategoryResponse>(request);
        
        return Ok($"Category with id {response.Message.CategoryId} has been added");
    }
    
    [HttpPut]
    [Route("category/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Models.Request.Category category)
    {
        var request = new UpdateCategoryRequest
        {
            CategoryId = id,
            Name = category.Name,
            Description = category.Description
        };
        
        var response = await updateCategoryClient.GetResponse<UpdateCategoryResponse>(request);
        
        if (!response.Message.Success)
        {
            return NotFound($"Category with id {id} not found");
        }
        
        return Ok($"Category with id {id} has been updated");
    }
    
    [HttpDelete]
    [Route("category/{id}")]
    public async Task<IActionResult> DeleteCategory(int id, [FromBody] int? newCategoryId = null)
    {
        var request = new DeleteCategoryRequest
        {
            CategoryId = id,
            NewCategoryId = newCategoryId
        };
        var response = await deleteCategoryClient.GetResponse<DeleteCategoryResponse>(request);
        
        if (!response.Message.Success)
        {
            return NotFound($"Category with id {id} or category with id {newCategoryId} not found");
        }
        
        return Ok($"Category with id {id} has been deleted");
    }
}
