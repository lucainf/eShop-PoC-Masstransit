using MassTransit;
using Products.Entities;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;

namespace Products.Services.Masstransit;

public class ProductConsumer(ProductsContext productsContext) : IConsumer<DeleteProductRequest>, 
    IConsumer<AddProductRequest>,
    IConsumer<UpdateProductRequest>,
    IConsumer<GetPriceRequest>
{
    public async Task Consume(ConsumeContext<DeleteProductRequest> context)
    {
        var prod = await productsContext.Products.FindAsync(context.Message.ProductId);
        if (prod == null)
        {
            await context.RespondAsync(new DeleteProductResponse { Success = false });
            return;
        }
        
        productsContext.Products.Remove(prod);
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new DeleteProductResponse { Success = true });
    }

    public async Task Consume(ConsumeContext<AddProductRequest> context)
    {
        var prod = new Product
        {
            Name = context.Message.Name,
            Description = context.Message.Description,
            Price = context.Message.Price,
            CategoryId = context.Message.CategoryId
        };
        
        await productsContext.Products.AddAsync(prod);
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new AddProductResponse { ProductId = prod.Id });
    }

    public async Task Consume(ConsumeContext<UpdateProductRequest> context)
    {
        var prod = await productsContext.Products.FindAsync(context.Message.ProductId);
        if (prod == null)
        {
            await context.RespondAsync(new UpdateProductResponse { Success = false });
            return;
        }
        
        if (String.IsNullOrEmpty(context.Message.Name))
            prod.Name = context.Message.Name;
        
        if (context.Message.Price != null)
            prod.Price = context.Message.Price.Value;
        
        if (context.Message.CategoryId != null && productsContext.Categories.Find(context.Message.CategoryId.Value) != null)
            prod.CategoryId = context.Message.CategoryId.Value;
        
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new UpdateProductResponse { Success = true });
    }

    public async Task Consume(ConsumeContext<GetPriceRequest> context)
    {
        var prod = await productsContext.Products.FindAsync(context.Message.Id);
        if (prod == null)
        {
            await context.RespondAsync(new GetPriceResponse { Price = 0 });
            return;
        }
        
        await context.RespondAsync(new GetPriceResponse { Price = prod.Price });
    }
}