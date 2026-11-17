using Amazon;
using Amazon.Textract;
using Common;
using FairShareSolutions_PYMT.API;
using FairShareSolutions_PYMT.API.CustomDependencyInjection;
using FairShareSolutions_PYMT.API.SignalR;
using FairShareSolutions_PYMT.Infrastructure.DataModel;
using FairShareSolutions_PYMT.Infrastructure.DbContext;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Forwarded headers: lets the app know the original request was HTTPS
// when SSL is terminated by a proxy / load balancer in front of IIS.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders =
		ForwardedHeaders.XForwardedFor |
		ForwardedHeaders.XForwardedProto;

	// By default only localhost proxies are trusted; the production proxy is not localhost.
	options.KnownNetworks.Clear();
	options.KnownProxies.Clear();
});


var logFilePath = ConstantBal.LogTextFile;

var logDirectory = Path.GetDirectoryName(logFilePath);

if (!string.IsNullOrWhiteSpace(logDirectory))
{
	Directory.CreateDirectory(logDirectory);
}

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console(
		outputTemplate: ConstantBal.LogOutputTemplate)
	.WriteTo.File(
		logFilePath,
		outputTemplate: ConstantBal.LogOutputTemplate,
		rollingInterval: RollingInterval.Day,
		retainedFileCountLimit: 5)
	.CreateLogger();


builder.Services.CustomDIService();
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString(ConstantBal.DefaultConnection),
		sqlServerOptionsAction: sqlOptions =>
		{
			// Enables automatic retries for transient connection errors
			sqlOptions.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(30),
				errorNumbersToAdd: null);
		})
	.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	var jwtSecret = builder.Configuration[ConstantBal.JWTSecret];
	var jwtIssuer = builder.Configuration[ConstantBal.JWTIssuer];
	var jwtAudience = builder.Configuration[ConstantBal.JWTAudience];

	if (string.IsNullOrWhiteSpace(jwtSecret))
	{
		throw new InvalidOperationException("JWT Secret is not configured.");
	}

	if (string.IsNullOrWhiteSpace(jwtIssuer))
	{
		throw new InvalidOperationException("JWT Issuer is not configured.");
	}

	if (string.IsNullOrWhiteSpace(jwtAudience))
	{
		throw new InvalidOperationException("JWT Audience is not configured.");
	}

	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = jwtIssuer,

		ValidateAudience = true,
		ValidAudience = jwtAudience,

		ValidateLifetime = true,

		ValidateIssuerSigningKey = true,

		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

		ClockSkew = TimeSpan.FromMinutes(1),

		AlgorithmValidator = (algorithm, securityKey, securityToken, validationParameters) =>
		{
			return algorithm == SecurityAlgorithms.HmacSha256;
		}
	};

	options.Events = new JwtBearerEvents
	{
		OnTokenValidated = async context =>
		{
			var tokenId = context.Principal?
				.FindFirst(JwtRegisteredClaimNames.Jti)?
				.Value;

			if (string.IsNullOrEmpty(tokenId))
			{
				context.Fail("Token id missing");
				return;
			}

			var tokenUserId = context.Principal?
				.FindFirst(ClaimTypes.Sid)?
				.Value;

			if (string.IsNullOrEmpty(tokenUserId))
			{
				context.Fail("User id missing in token");
				return;
			}

			var dbContext = context.HttpContext
				.RequestServices
				.GetRequiredService<ApplicationDbContext>();

			var userToken = await dbContext.UserToken.AsNoTracking()
				.FirstOrDefaultAsync(x => x.TokenId == tokenId);

			if (userToken == null)
			{
				context.Fail("Token not found");
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.Response.ContentType = "application/json";

				var response = new ApplicationResponse
				{
					Message = FairShareResources.UnAuthorizedAccess,
					Data = null,
					StatusCode = StatusCodes.Status401Unauthorized,
					Status = false
				};

				await context.Response.WriteAsync(
					JsonSerializer.Serialize(response));

				return;
			}

			// Compare the userId received in token and the user id stored in database (on behalf of tokenId).
			if (!string.Equals(userToken.UserId, tokenUserId, StringComparison.OrdinalIgnoreCase))
			{
				context.Fail("Token does not belong to this user");
				return;
			}

			if (userToken.IsRevoked)
			{
				context.Fail("Token revoked");
				return;
			}

			if (userToken.ExpiryDate <= DateTime.UtcNow)
			{
				context.Fail("Token expired");
				return;
			}
		},

		OnChallenge = async context =>
		{
			// Prevent default 401 response
			context.HandleResponse();

			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Response.ContentType = "application/json";

			var response = new ApplicationResponse
			{
				Message = FairShareResources.UnAuthorizedAccess,
				Data = null,
				StatusCode = StatusCodes.Status401Unauthorized,
				Status = ConstantBal.False
			};

			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	};
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc(ConstantBal.V1, new OpenApiInfo { Title = ConstantBal.MyAPI, Version = ConstantBal.V1 });

	// Add security definition for Bearer token
	c.AddSecurityDefinition(ConstantBal.BearerCap, new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = ConstantBal.BearerMessage,
		Name = ConstantBal.Authorization,
		Type = SecuritySchemeType.Http,
		Scheme = ConstantBal.BearerSmall,
		BearerFormat = ConstantBal.JWT,
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = ConstantBal.BearerCap
		}
	});

	// Add security requirement
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = ConstantBal.BearerCap
				}
			},
			new string[] {}
		}
	});
});

builder.Services.Configure<AppleWalletCredentialModel>(builder.Configuration.GetSection(ConstantBal.AppleWalletCredential));

// Configure AWS Textract client
builder.Services.AddSingleton<IAmazonTextract>(sp =>
{
	var config = new AmazonTextractConfig
	{
		RegionEndpoint = RegionEndpoint.USEast2
	};
	return new AmazonTextractClient(builder.Configuration[ConstantBal.AWSAccessKey], builder.Configuration[ConstantBal.AWSSecretKey], config);
});


// CORS: origins must NOT have a trailing slash.
// The policy name uses the same constant as app.UseCors(...) below, so they always match.
builder.Services.AddCors(options =>
{
	options.AddPolicy(ConstantBal.NewCorsPolicy, policy =>
	{
		policy.WithOrigins(
				"https://fairsharesolutions-pymt-api.chetu.com",
				"https://localhost:5294",
				"https://usefairshare.com",
				"https://api.usedivity.com")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});


var usioSection = builder.Environment.IsProduction()
	? builder.Configuration.GetSection("USIOProduction")
	: builder.Configuration.GetSection("USIO");

var secretManager = builder.Environment.IsProduction()
	? builder.Configuration.GetSection("SecretCredentialsProduction")
	: builder.Configuration.GetSection("SecretCredentials");

builder.Services.Configure<UsioSettings>(usioSection);

builder.Services.Configure<SecretCredentials>(secretManager);

Log.Information("Application Environment: {Environment}", builder.Environment.EnvironmentName);

Log.Information("USIO Configuration Selected: {USIOSection}", builder.Environment.IsProduction() ? "USIOProduction" : "USIO");
Log.Information("Secret Configuration Selected: {SecretCongfig}", builder.Environment.IsProduction() ? "SecretCredentialsProduction" : "SecretCredentials");

builder.Services.Configure<AkimboSettings>(builder.Configuration.GetSection("AkimboGiftCard"));

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// MUST be the first middleware so every later step sees the real scheme (https).
app.UseForwardedHeaders();

// Map the Hub
app.MapHub<GetItems>(ConstantBal.GetItems);


// Swagger / ReDoc (enabled in all environments, as before)
app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc(c =>
{
	c.SpecUrl("/swagger/v1/swagger.json");
	c.RoutePrefix = "redoc";
	c.DocumentTitle = "API Documentation";
});

// HTTPS redirect only locally. In production HTTPS is enforced in front of IIS;
// redirecting here caused the infinite 307 loop.
if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseSession();

app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
	{
		context.Response.ContentType = ConstantBal.ApplicationJson;

		var response = new
		{
			message = ConstantBal.UnAuthorizedMessage,
			data = (object?)null,
			statusCode = StatusCodes.Status401Unauthorized,
			status = false
		};

		await context.Response.WriteAsync(
			JsonSerializer.Serialize(response)
		);
	}
});

app.UseCors(ConstantBal.NewCorsPolicy);

app.UseStaticFiles(new StaticFileOptions
{
	ServeUnknownFileTypes = true,
	DefaultContentType = ConstantBal.ApplicationJson
});
app.UseDefaultFiles();

app.UseRouting();
app.UseRewriter(
	new RewriteOptions()
		.AddRewrite("^apple-app-site-association$", "/apple-app-site-association.json", true)
);

app.UseAuthentication();
app.UseMiddleware<UserAuthorizationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

// MVC route
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Plaid}/{action=CallBack}/{id?}");
app.MapScalarApiReference(options =>
{
	options.Title = "FairShare API Documentation";
});
app.MapControllers();


app.Run();