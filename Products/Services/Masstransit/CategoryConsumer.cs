using MassTransit;
using Products.Entities;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;

namespace Products.Services.Masstransit;

public class CategoryConsumer(ProductsContext productsContext) : IConsumer<AddCategoryRequest>, 
    IConsumer<UpdateCategoryRequest>,
    IConsumer<DeleteCategoryRequest>
{
    public async Task Consume(ConsumeContext<AddCategoryRequest> context)
    {
        var category = new Category
        {
            Name = context.Message.Name,
            Description = context.Message.Description
        };
        await productsContext.Categories.AddAsync(category);
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new AddCategoryResponse() { CategoryId = category.Id });
    }

    public async Task Consume(ConsumeContext<UpdateCategoryRequest> context)
    {
        var category = await productsContext.Categories.FindAsync(context.Message.CategoryId);
        if (category == null)
        {
            await context.RespondAsync(new UpdateCategoryResponse() { Success = false });
            return;
        }
        
        if (!string.IsNullOrEmpty(context.Message.Name))
            category.Name = context.Message.Name;
        if (!string.IsNullOrEmpty(context.Message.Description))
            category.Description = context.Message.Description;
        
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new UpdateCategoryResponse() { Success = true });
    }

    public async Task Consume(ConsumeContext<DeleteCategoryRequest> context)
    {
        var oldCategory = await productsContext.Categories.FindAsync(context.Message.CategoryId);
        var newCategory = await productsContext.Categories.FindAsync(context.Message.NewCategoryId);
        if (oldCategory == null || (oldCategory.Products.Any() && newCategory == null))
        {
            await context.RespondAsync(new DeleteCategoryResponse() { Success = false });
            return;
        }
        
        foreach (var p in oldCategory.Products)
        {
            p.Category = newCategory;
        }
        
        productsContext.Categories.Remove(oldCategory);
        await productsContext.SaveChangesAsync();
        await context.RespondAsync(new DeleteCategoryResponse() { Success = true });
    }
}