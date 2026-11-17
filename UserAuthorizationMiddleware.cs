using Common;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Model.IRepository;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Text.Json;

public class UserAuthorizationMiddleware
{
	private readonly RequestDelegate _next;

	public UserAuthorizationMiddleware(RequestDelegate next)
	{
		_next = next;
	}


	//private readonly string[] userKeywords =
	//{
	//	"userid",
	//	"user_id",
	//	"user",
	//};

	private static readonly HashSet<string> ExactUserIdKeys =
	new(StringComparer.OrdinalIgnoreCase)
	{
			"userId",
			"user_id",
			"user-id",
			"user",
			"adminId"
	};

	private static readonly HashSet<string> ExactGroupIdKeys =
	new(StringComparer.OrdinalIgnoreCase)
	{
		"groupId",
		"group_id",
		"group-id",
		"group"
	};


	public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
	{
		var endpoint = context.GetEndpoint();
		if (context.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
		{
			await _next(context);
			return;
		}

		if (endpoint == null)
		{
			await _next(context);
			return;
		}

		var anonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
		if (anonymous != null)
		{
			await _next(context);
			return;
		}

		// ---------------------------------------------------------
		// BYPASS USER AUTHORIZATION FOR GetAllSplitData
		// ---------------------------------------------------------
		if (context.Request.Path.Equals("/api/Group/GetAllSplitData", StringComparison.OrdinalIgnoreCase) ||
			context.Request.Path.Equals("/hubs/getitems", StringComparison.OrdinalIgnoreCase))
		{
			await _next(context);
			return;
		}

		//if (!context.User.Identity?.IsAuthenticated ?? true)
		//{
		//	await WriteJsonError(context, StatusCodes.Status403Forbidden);
		//	return;
		//}
		if (!(context.User.Identity?.IsAuthenticated ?? true))
		{
			Log.Warning($"Authentication failed. Endpoint:{endpoint}, StatusCode:{StatusCodes.Status401Unauthorized}");
			await WriteJsonError(context, StatusCodes.Status401Unauthorized);
			return;
		}

		var currentUserId = currentUserService.GetCurrentUserId();

		if (string.IsNullOrEmpty(currentUserId))
		{
			Log.Warning($"Authorization denied. UserId not found, Endpoint:{endpoint}, StatusCode:{StatusCodes.Status401Unauthorized}");
			await WriteJsonError(context, StatusCodes.Status403Forbidden);
			return;

		}

		// Collect User and Group IDs from request
		var requestedIds = new List<string>();
		var requestedGroupIds = new List<string>();

		foreach (var item in context.Request.Query)
		{
			// Get User Id's
			if (ExactUserIdKeys.Contains(item.Key))
			{
				foreach (var value in item.Value)
				{
					if (!string.IsNullOrEmpty(value))
					{
						requestedIds.Add(value);
					}
				}
			}

			// Get Groupp ID's
			if (ExactGroupIdKeys.Contains(item.Key))
			{
				foreach (var value in item.Value)
				{
					if (!string.IsNullOrEmpty(value))
					{
						requestedGroupIds.Add(value);
					}
				}
			}
		}


		// Query String
		//foreach (var item in context.Request.Query)
		//{
		//	if (IsUserRelatedKey(item.Key))
		//	{
		//		requestedIds.Add(item.Value);
		//	}
		//}


		foreach (var item in context.Request.RouteValues)
		{
			var value = item.Value?.ToString();

			if (string.IsNullOrEmpty(value))
			{
				continue; 
			}

			if (ExactUserIdKeys.Contains(item.Key))
			{
				requestedIds.Add(value);
			}

			if (ExactGroupIdKeys.Contains(item.Key))
			{
				requestedGroupIds.Add(value);
			}
		}

		// Route Values
		//foreach (var item in context.Request.RouteValues)
		//{
		//	if (IsUserRelatedKey(item.Key))
		//	{
		//		requestedIds.Add(item.Value?.ToString());
		//	}
		//}

		// Request Body
		if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
		{
			context.Request.EnableBuffering();

			using var reader = new StreamReader(context.Request.Body, leaveOpen: true);

			var body = await reader.ReadToEndAsync();

			context.Request.Body.Position = 0;

			if (!string.IsNullOrWhiteSpace(body))
			{
				try
				{
					var json = JsonSerializer.Deserialize<JsonElement>(body);

					ExtractUserIdsFromJson(json, requestedIds, requestedGroupIds);

				}
				catch (Exception)
				{
					throw;
				}

			}

		}

		if (context.Request.ContentType != null && context.Request.ContentType.Contains("multipart/form-data"))
		{
			// Safely access the form data parsed by ASP.NET Core
			var form = await context.Request.ReadFormAsync();

			// Iterate through all text keys in the form
			foreach (var key in form.Keys)
			{
				var formValue = form[key].ToString();

				if (!string.IsNullOrWhiteSpace(formValue))
				{
					// Check if the individual form field contains a JSON string
					if (formValue.TrimStart().StartsWith("{") || formValue.TrimStart().StartsWith("["))
					{
						try
						{
							var json = JsonSerializer.Deserialize<JsonElement>(formValue);
							ExtractUserIdsFromJson(json, requestedIds, requestedGroupIds);
						}
						catch (JsonException)
						{
							// The field starts like JSON but failed parsing; handle or extract as a plain string instead
							ExtractIdsFromPlainString(key, formValue, requestedIds, requestedGroupIds);
						}
					}
					else
					{
						// Handle regular text fields (e.g., userId=123 or groupIds=1,2,3)
						ExtractIdsFromPlainString(key, formValue, requestedIds, requestedGroupIds);
					}
				}
			}
		}

		Log.Information($"Authorization request. UserId:{currentUserId}, Endpoint:{endpoint}, UserIds:[{string.Join(",", requestedIds)}], GroupIds:[{string.Join(",", requestedGroupIds)}]");

		var isAuthorized = await currentUserService.ValidateUserAuthorizationAsync(
				currentUserId,
				requestedIds,
				requestedGroupIds);

		Log.Information($"Authorization result. UserId:{currentUserId}, Endpoint:{endpoint}, Validated:{isAuthorized}");

		if (!isAuthorized)
		{
			Log.Warning($"Authorization denied. UserId:{currentUserId}, Endpoint:{endpoint}, UserIds:[{string.Join(",", requestedIds)}], GroupIds:[{string.Join(",", requestedGroupIds)}], StatusCode:{StatusCodes.Status403Forbidden}");
			await WriteJsonError(context, StatusCodes.Status403Forbidden);
			return;
		}


		// Compare
		//foreach (var requestUserId in requestedIds)
		//{
		// Validate User / Group Authorization


		//if (string.IsNullOrEmpty(requestUserId))
		//	continue;
		//
		////	if (requestUserId != currentUserId)
		//if (!string.Equals(requestUserId, currentUserId, StringComparison.OrdinalIgnoreCase))
		//{
		//	await WriteJsonError(context, StatusCodes.Status403Forbidden);
		//	return;
		//}

		//}
		Log.Information(
	$"Authorization passed. UserId:{currentUserId}, Endpoint:{endpoint}, Request forwarded");
		await _next(context);

	}


	private void ExtractUserIdsFromJson(JsonElement element, List<string> ids, List<string> groupIds)
	{
		if (element.ValueKind == JsonValueKind.Object)
		{
			foreach (var property in element.EnumerateObject())
			{
				// =========================
				// USER / ADMIN ID
				// =========================
				if (ExactUserIdKeys.Contains(property.Name))
				{
					string? value = GetJsonValueAsString(property.Value);

					if (!string.IsNullOrWhiteSpace(value))
					{
						ids.Add(value);
					}
				}

				// =========================
				// GROUP ID
				// =========================
				if (ExactGroupIdKeys.Contains(property.Name))
				{
					string? value = GetJsonValueAsString(property.Value);

					if (!string.IsNullOrWhiteSpace(value))
					{
						groupIds.Add(value);
					}
				}

				// Recursively check nested objects/arrays
				ExtractUserIdsFromJson(
					property.Value,
					ids,
					groupIds);
			}
		}
		else if (element.ValueKind == JsonValueKind.Array)
		{
			foreach (var item in element.EnumerateArray())
			{
				ExtractUserIdsFromJson(
					item,
					ids,
					groupIds);
			}
		}
	}

	private string? GetJsonValueAsString(JsonElement element)
	{
		return element.ValueKind switch
		{
			JsonValueKind.String => element.GetString(),

			JsonValueKind.Number => element.GetRawText(),

			JsonValueKind.True => "true",

			JsonValueKind.False => "false",

			_ => null
		};
	}
	private static async Task WriteJsonError(HttpContext context, int statusCode)
	{
		context.Response.StatusCode = statusCode;
		context.Response.ContentType = "application/json";

		var response = new ApplicationResponse
		{
			Message = FairShareResources.UnAuthorizedAccess,
			Data = null,
			StatusCode = statusCode,
			Status = false
		};

		await context.Response.WriteAsync(JsonSerializer.Serialize(response));
	}

	private void ExtractIdsFromPlainString(string key, string value, List<string> requestedIds, List<string> requestedGroupIds)
	{
		// Adjust key names to match your expected form field names
		if (key.Equals("userId", StringComparison.OrdinalIgnoreCase) || key.Equals("userIds", StringComparison.OrdinalIgnoreCase))
		{
			var ids = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
			requestedIds.AddRange(ids.Select(id => id.Trim()));
		}
		else if (key.Equals("groupId", StringComparison.OrdinalIgnoreCase) || key.Equals("groupIds", StringComparison.OrdinalIgnoreCase))
		{
			var ids = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
			requestedGroupIds.AddRange(ids.Select(id => id.Trim()));
		}
	}
}