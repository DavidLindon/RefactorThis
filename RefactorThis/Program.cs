using Microsoft.EntityFrameworkCore;

namespace RefactorThis {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			var productsConnectionString = builder.Configuration["Settings:ConnectionStrings:Products"] ?? throw new InvalidOperationException("Products connection string not set");
			builder.Services.AddDbContext<ProductsContext>(options => options.UseSqlServer(productsConnectionString));
			var logger = builder.Logging.AddConsole();
			
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment()) {
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.ConfigureExceptionHandler(app.Logger);
			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}