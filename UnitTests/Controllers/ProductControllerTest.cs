using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefactorThis;
using RefactorThis.Controllers;
using RefactorThis.Models;
using System;
using System.Net;
using System.Web.Http;

namespace UnitTests {
	[TestClass]
	public class ProductControllerTests {

		[TestMethod]
		public async Task GetAllAsync() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			productsContext.Products.Add(new Product() { Name = "Test Product", Description = "A very interesting product!" });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductDTO[] productDTOs = await productController.GetAllAsync();

			// Assert
			Assert.AreEqual(1, productDTOs.Length);
			Assert.AreEqual("Test Product", productDTOs[0].Name);
			Assert.AreEqual("A very interesting product!", productDTOs[0].Description);
		}

		[TestMethod]
		public async Task SearchByNameAsync_HandlesMixedCase() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			productsContext.Products.Add(new Product() { Name = "Computer", Description = "A very interesting product!" });
			productsContext.Products.Add(new Product() { Name = "Monitor",Description = "A very interesting product!" });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductDTO[] productDTOs = await productController.SearchByNameAsync("computer");

			// Assert
			Assert.AreEqual(2,productsContext.Products.Count());
			Assert.AreEqual(1,productDTOs.Length);
			Assert.AreEqual("Computer",productDTOs[0].Name);
		}

		[TestMethod]
		public async Task SearchByNameAsync_Handlesnull() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			productsContext.Products.Add(new Product() { Name = "Computer",Description = "A very interesting product!" });
			productsContext.Products.Add(new Product() { Name = "Monitor",Description = "A very interesting product!" });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductDTO[] productDTOs = await productController.SearchByNameAsync(null);

			// Assert
			Assert.AreEqual(2,productsContext.Products.Count());
			Assert.AreEqual(0,productDTOs.Length);
		}

		[TestMethod]
		public async Task GetProduct() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			productsContext.Products.Add(new Product() { Id = guid, Name = "Computer",Description = "A very interesting product!" });
			productsContext.Products.Add(new Product() { Id = Guid.NewGuid(),Name = "Computer",Description = "A very interesting product!" });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductDTO productDTO = await productController.GetProductAsync(guid);

			// Assert
			Assert.AreEqual(2,productsContext.Products.Count());
			Assert.AreEqual(guid,productDTO.Id);
		}

		[TestMethod]
		public async Task GetProduct_HandlesNonExistant() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);

			// Assert
			try {
				ProductDTO productDTO = await productController.GetProductAsync(Guid.NewGuid());
				Assert.Fail("Failed to throw exception");
			} catch (HttpResponseException ex) {
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			} catch {
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task CreateAsync() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			await productController.CreateAsync(new Product() { Name = "Computer",Description = "A very interesting product!" });

			// Assert
			Assert.AreEqual(1,productsContext.Products.Count());
			Assert.AreEqual("Computer",productsContext.Products.ToArray()[0].Name);
		}

		[TestMethod]
		public async Task UpdateAsync() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			Product product = new Product() { Id = guid, Name = "Computer",Description = "A very interesting product!" };
			productsContext.Products.Add(product);
			await productsContext.SaveChangesAsync();
			productsContext.ChangeTracker.Clear();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			await productController.UpdateAsync(new Product() { Id = guid,Name = "New Computer",Description = "A even better one" });

			// Assert
			Assert.AreEqual("New Computer",productsContext.Products.First().Name);
			Assert.AreEqual("A even better one",productsContext.Products.First().Description);
		}

		[TestMethod]
		public async Task UpdateAsync_HandlesTryingToUpdateProductThatDoesntExist() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();

			try {
				ProductsController productController = new ProductsController(null,productsContext);
				await productController.UpdateAsync(new Product() { Id = Guid.NewGuid(),Name = "Computer",Description = "A very interesting product!" });
				Assert.Fail("Failed to throw exception");
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			}
			catch
			{
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task Delete() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			productsContext.Products.Add(new Product() { Id = guid,Name = "Computer",Description = "A very interesting product!" });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			await productController.DeleteAsync(guid);

			// Assert
			Assert.AreEqual(0,productsContext.Products.Count());
		}

		[TestMethod]
		public async Task Delete_HandlesNonExistant() {
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);

			// Assert
			try {
				ProductDTO productDTO = await productController.GetProductAsync(Guid.NewGuid());
				Assert.Fail("Failed to throw exception");
			} catch (HttpResponseException ex) {
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			} catch {
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task GetOptionsAsync()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			Product parentProduct = new Product() { Id = guid,Name = "ProductName",Description = "Desc" };
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 1",Description = "A very interesting option!",Product = parentProduct });
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 2",Description = "A very interesting option!",Product = parentProduct });
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 3",Description = "A very interesting option!",Product = new Product() { Id = Guid.NewGuid(),Name = "ProductName",Description = "Desc" } });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductOptionDTO[] productOptions = await productController.GetOptionsAsync(guid);

			// Assert
			Assert.AreEqual(2,productOptions.Length); ;
		}

		[TestMethod]
		public async Task GetOptionAsync()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			Product parentProduct = new Product() { Id = Guid.NewGuid(),Name = "ProductName",Description = "Desc" };
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 1",Description = "A very interesting option!",Product = parentProduct });
			productsContext.ProductOptions.Add(new ProductOption() { Id = guid,Name = "Option 2",Description = "A very interesting option!",Product = parentProduct });
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 3",Description = "A very interesting option!",Product = new Product() { Id = Guid.NewGuid(),Name = "ProductName",Description = "Desc" } });
			await productsContext.SaveChangesAsync();

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			ProductOptionDTO productOption = await productController.GetOptionAsync(Guid.NewGuid(), guid);

			// Assert
			Assert.AreEqual("Option 2",productOption.Name); ;
		}

		[TestMethod]
		public async Task GetOptionAsync_HandlesNonExistant()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			ProductsController productController = new ProductsController(null,productsContext);

			try
			{
				await productController.GetOptionAsync(Guid.NewGuid(),Guid.NewGuid());
				Assert.Fail("Failed to throw exception");
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			}
			catch
			{
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task CreateOptionAsync()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			Product parentProduct = new Product() { Id = Guid.NewGuid(),Name = "ProductName",Description = "Desc" };
			productsContext.ProductOptions.Add(new ProductOption() { Id = Guid.NewGuid(),Name = "Option 1",Description = "A very interesting option!",Product = parentProduct });
			await productsContext.SaveChangesAsync();
			Assert.AreEqual(1,productsContext.ProductOptions.Count());

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			await productController.CreateOptionAsync(parentProduct.Id,new ProductOption() { Name = "Option 1",Description = "A very interesting option!" });

			// Assert
			Assert.AreEqual(2,productsContext.ProductOptions.Count());
		}

		[TestMethod]
		public async Task CreateOptionAsync_HandlesNonExistant()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			ProductsController productController = new ProductsController(null,productsContext);

			try
			{
				await productController.CreateOptionAsync(Guid.NewGuid(), null);
				Assert.Fail("Failed to throw exception");
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			}
			catch
			{
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task CreateOptionAsync_HandlesAlreadyExistingProductOption()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			Product parentProduct = new Product() { Id = Guid.NewGuid(),Name = "ProductName",Description = "Desc" };
			ProductOption productOption = new ProductOption() { Id = Guid.NewGuid(),Name = "Option 1",Description = "A very interesting option!",Product = parentProduct };
			productsContext.ProductOptions.Add(productOption);
			await productsContext.SaveChangesAsync();
			ProductsController productController = new ProductsController(null,productsContext);

			try
			{
				await productController.CreateOptionAsync(parentProduct.Id, productOption);
				Assert.Fail("Failed to throw exception");
			}
			catch (InvalidOperationException ex)
			{
				Assert.AreEqual("An option with this id already exists",ex.Message);
			}
			catch
			{
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		[TestMethod]
		public async Task DeleteOptionAsync()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			Guid guid = Guid.NewGuid();
			productsContext.ProductOptions.Add(new ProductOption() { Id = guid,Name = "Option 1",Description = "A very interesting option!"});
			await productsContext.SaveChangesAsync();
			Assert.AreEqual(1,productsContext.ProductOptions.Count());

			// Act
			ProductsController productController = new ProductsController(null,productsContext);
			await productController.DeleteOptionAsync(guid);

			// Assert
			Assert.AreEqual(0,productsContext.ProductOptions.Count());
		}

		[TestMethod]
		public async Task DeleteOptionAsync_HandlesNonExistant()
		{
			// Arange
			ProductsContext productsContext = NewInMemoryProductContext();
			ProductsController productController = new ProductsController(null,productsContext);

			try
			{
				await productController.DeleteOptionAsync(Guid.NewGuid());
				Assert.Fail("Failed to throw exception");
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound,ex.Response.StatusCode);
			}
			catch
			{
				Assert.Fail("Incorrect exception type thrown");
			}
		}

		public ProductsContext NewInMemoryProductContext() {
			return new ProductsContext(new DbContextOptionsBuilder<ProductsContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
		}
	}
}