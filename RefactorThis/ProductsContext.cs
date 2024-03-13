using Microsoft.EntityFrameworkCore;
using RefactorThis.Models;

namespace RefactorThis {
	public class ProductsContext : DbContext {
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductOption> ProductOptions { get; set; }
		public ProductsContext() {

		}

		public ProductsContext(DbContextOptions<ProductsContext> options) : base(options) { 
		
		}
	}
}
