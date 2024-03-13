using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefactorThis.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace RefactorThis.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ProductsController :ControllerBase
	{
		private readonly ILogger<ProductsController> _logger;
		private readonly ProductsContext _productsContext;

		public ProductsController(ILogger<ProductsController> logger,ProductsContext productsContext)
		{
			_logger = logger;
			_productsContext = productsContext;
		}

		[HttpGet("GetAll")]
		public async Task<ProductDTO[]> GetAllAsync()
		{
			// For product we could implement some caching
			return await _productsContext.Products.Select(x => new ProductDTO(x)).ToArrayAsync();
		}

		[HttpGet("SearchByName")]
		public async Task<ProductDTO[]> SearchByNameAsync(string name)
		{
			// I'm assuming products cannot have null names, so if we get a search request for null, just return an empty array
			if (name == null)
			{
				return new ProductDTO[0];
			}
			return await _productsContext.Products.Where(x => x.Name != null && x.Name.ToLower().Contains(name)).Select(x => new ProductDTO(x)).ToArrayAsync();
		}

		[Route("{id}")]
		[HttpGet("GetProduct")]
		public async Task<ProductDTO> GetProductAsync(Guid id)
		{
			// For product we could implement some caching
			Product product = await _productsContext.Products.FirstOrDefaultAsync(x => x.Id == id);
			if (product == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			return new ProductDTO(product);
		}

		[HttpPost("Create")]
		public async Task CreateAsync(Product product)
		{
			await _productsContext.Products.AddAsync(product);
			await _productsContext.SaveChangesAsync();
		}

		[Route("{id}")]
		[HttpPut("Update")]
		public async Task UpdateAsync(Product product)
		{
			Product existingProduct = await _productsContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.Id);
			if (existingProduct == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			_productsContext.Update(product);
			int recordsAffected = await _productsContext.SaveChangesAsync();
			if (recordsAffected != 1)
			{
				throw new InvalidOperationException("No records updated");
			}
		}

		[Route("{id}")]
		[HttpDelete("Delete")]
		public async Task DeleteAsync(Guid guid)
		{
			// This assumes that on delete is set to Cascade in the databse, otherwise it would be necessary to delete all the child options before the parent.
			Product product = await _productsContext.Products.FirstOrDefaultAsync(x => x.Id == guid);
			if (product == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			_productsContext.Entry(product).State = EntityState.Deleted;
			await _productsContext.SaveChangesAsync();
		}

		[Route("{id}/options")]
		[HttpGet("Options")]
		public async Task<ProductOptionDTO[]> GetOptionsAsync(Guid productId)
		{
			return await _productsContext.ProductOptions.Where(x => x.Product.Id == productId).Select(x=> new ProductOptionDTO(x)).ToArrayAsync();
		}

		[Route("{productId}/options/{id}")]
		[HttpGet("Options")]
		public async Task<ProductOptionDTO> GetOptionAsync(Guid productId,Guid id)
		{
			ProductOption productOption = await _productsContext.ProductOptions.FirstOrDefaultAsync(x=>x.Id == id);
			if (productOption == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			return new ProductOptionDTO(productOption);
		}

		[Route("{productId}/options")]
		[HttpPost]
		public async Task CreateOptionAsync(Guid productId,ProductOption productOption)
		{
			Product product = await _productsContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
			if (productOption == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			if (product.Options.Any(x => x.Id == productOption.Id))
			{
				throw new InvalidOperationException("An option with this id already exists");
			}
			product.Options.Add(productOption);
			await _productsContext.SaveChangesAsync();
		}

		[Route("{productId}/options/{id}")]
		[HttpPut]
		public async Task UpdateOptionAsync(Guid id,ProductOption option)
		{
			_productsContext.Update(option);
			int recordsAffected = await _productsContext.SaveChangesAsync();
			if (recordsAffected != 1)
			{
				throw new InvalidOperationException("No records updated");
			}
		}

		[Route("{productId}/options/{id}")]
		[HttpDelete]
		public async Task DeleteOptionAsync(Guid id)
		{
			ProductOption productOption = await _productsContext.ProductOptions.FindAsync(id);
			if (productOption == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			_productsContext.Entry(productOption).State = EntityState.Deleted;
			int recordsAffected = await _productsContext.SaveChangesAsync();
			if (recordsAffected != 1)
			{
				throw new InvalidOperationException("No records updated");
			}
		}
	}
}