using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace RefactorThis {
	public static class ExceptionMiddlewareExtensions {
		public static void ConfigureExceptionHandler(this IApplicationBuilder application, ILogger logger) {
			application.UseExceptionHandler(appError => {
				appError.Run(async context => {
					if (context.Request.Headers["Accept"].ToString().Contains("application/json")) {
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
						context.Response.ContentType = "application/json";
						var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
						if (contextFeature != null) {
							logger?.LogError(contextFeature.Error.ToString());
							await context.Response.WriteAsync(JsonConvert.SerializeObject(contextFeature.Error));
						}
					}
				});
			});
		}
	}
}
