using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Products.Controllers;
using Products.Entities;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class ProductsControllerTests
{
    private ProductsContext _productsContext;
    private ProductsController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _productsContext = new ProductsContext(options);
        _controller = new ProductsController(_productsContext);
    }

    [TearDown]
    public void TearDown()
    {
        _productsContext.Database.EnsureDeleted();
        _productsContext.Dispose();
    }

    [Test]
    public void GetProduct_InvalidCategory_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetProduct(999);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void GetProduct_ValidCategory_ReturnsProducts()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Electronics", Description = "Electronic items" };
        _productsContext.Categories.Add(category);
        _productsContext.Products.Add(new Product { Id = 1, Name = "Laptop", CategoryId = 1, Description  = "Laptop description", Price = 1000});
        _productsContext.Products.Add(new Product { Id = 2, Name = "Smartphone", CategoryId = 1, Description = "Smartphone description", Price = 500});
        _productsContext.SaveChanges();

        // Act
        var result = _controller.GetProduct(1) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var products = result.Value as IEnumerable<Product>;
        Assert.AreEqual(2, products.Count());
    }

    [Test]
    public void GetProducts_ReturnsAllProducts()
    {
        // Arrange
        _productsContext.Products.Add(new Product { Id = 1, Name = "Laptop", CategoryId = 1, Description  = "Laptop description", Price = 1000});
        _productsContext.Products.Add(new Product { Id = 2, Name = "Smartphone", CategoryId = 1, Description = "Smartphone description", Price = 500});
        _productsContext.SaveChanges();

        // Act
        var result = _controller.GetProducts() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var products = result.Value as IEnumerable<Product>;
        Assert.AreEqual(2, products.Count());
    }
}