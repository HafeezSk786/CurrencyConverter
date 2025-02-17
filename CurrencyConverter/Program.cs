using CurrencyConverter.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Serilog;
using Serilog.Core;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "issuer",
            ValidAudience = "audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ASFSDFWT55dgdj43p543535gegfdgdsfsdfgdgh"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});
//Caching
builder.Services.AddOutputCache();
builder.Services.AddTransient<ICurrencyProvider, FrankfurterService>();
//Resilience pattern (retry mechanisms, Exponential Backoff, circuit breaker
builder.Services.AddResiliencePipeline("default", builder =>
{
    builder.AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>(),
        // Retry delay
        Delay = TimeSpan.FromSeconds(2),
        // Maximum retries           
        MaxRetryAttempts = 3,
        // Exponential backoff                   
        BackoffType = DelayBackoffType.Constant,
        // Adds jitter to reduce collision
        UseJitter = true
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        // Customize and configure the circuit breaker logic.
        SamplingDuration = TimeSpan.FromSeconds(3),
        FailureRatio = 0.7,
        MinimumThroughput = 2,
        ShouldHandle = static args =>
        {
            return ValueTask.FromResult(args is
            {
                Outcome.Result: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
            });
        }
    })
    // Timeout after 15 seconds  
   .AddTimeout(TimeSpan.FromSeconds(15));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//api throttling can be achieved by adding the custom middleware
app.UseMiddleware<RateLimitingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//Caching
app.UseOutputCache();
app.UseSerilogRequestLogging(); //logging
app.Run();
