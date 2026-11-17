using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using Serilog;
using System.Net;

namespace FairShareSolutions_PYMT.API
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		//public async Task InvokeAsync(HttpContext context)
		//{
		//	try
		//	{
		//		Log.Information($"Global Exception Middleware calls for Request: {context.GetEndpoint()?.DisplayName ?? "Endpoint not resolved"}"); 
		//		await _next(context);
		//	}
		//	catch (Exception ex)
		//	{
		//		var endpoint = context.GetEndpoint()?.DisplayName ?? "Unknown Endpoint";
		//		var method = context.Request.Method;
		//		var path = context.Request.Path;
		//		var queryString = context.Request.QueryString.ToString();

		//		Log.Error(ex, $"Unhandled exception occurred. Endpoint: {endpoint}, Method: {method}, Path: {path}, QueryString: {queryString}, Exception: {ex.Message}");

		//		_logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
		//		await HandleExceptionAsync(context, ex);
		//	}
		//}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				Log.Information(
					"Global Exception Middleware called | Method: {Method} | Path: {Path} | Endpoint: {Endpoint} | Authenticated: {Authenticated} | UserId: {UserId}",
					context.Request.Method,
					context.Request.Path,
					context.GetEndpoint()?.DisplayName ?? "Endpoint not resolved",
					context.User.Identity?.IsAuthenticated ?? false,
					context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
						?? context.User.FindFirst("sid")?.Value
						?? "Anonymous");

				await _next(context);

				Log.Information(
					"Request completed | Method: {Method} | Path: {Path} | StatusCode: {StatusCode}",
					context.Request.Method,
					context.Request.Path,
					context.Response.StatusCode);
			}
			catch (Exception ex)
			{
				var endpoint = context.GetEndpoint()?.DisplayName ?? "Unknown Endpoint";
				var method = context.Request.Method;
				var path = context.Request.Path;
				var queryString = context.Request.QueryString.ToString();

				Log.Error(ex, "Unhandled exception occurred. Endpoint: {Endpoint}, Method: {Method}, Path: {Path}, QueryString: {QueryString}, Exception: {Exception}",
					endpoint, method, path, queryString, ex.Message);

				_logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

				await HandleExceptionAsync(context, ex);
			}
		}


		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			return context.Response.WriteAsync(new ErrorDetails()
			{
				StatusCode = context.Response.StatusCode,
				Message = "A technical error occurred. Please try again later.",
				TraceId = context.TraceIdentifier
			}.ToString());
		}
	}
}
