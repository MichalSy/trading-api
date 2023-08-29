using Amazon.CognitoIdentityProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using TradingApi;
using TradingApi.Authentication;
using TradingApi.Endpoints.ZeroApi;
using TradingApi.Endpoints.ZeroRealtime;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Manager.Storage.InstrumentStorage;
using TradingApi.Manager.Storage.OrderSignal;
using TradingApi.Manager.Storage.OrderSignalDetector;
using TradingApi.Manager.Storage.TradingStorage;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.ZeroApi;
using TradingApi.Repositories.ZeroRealtime;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("ZeroTrade", new OpenApiSecurityScheme()
    {
        Name = "zero-auth",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ZeroTrade",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "ZeroTrade",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "ZeroTrade",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ZeroTradeAuthenticationHandler>("ZeroTrade", null);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ZeroTrade", policy => policy.RequireAuthenticatedUser()
    .RequireClaim(ClaimTypes.Name, "ZeroTrade"));

builder.Services.AddMongoDb();

// Repository
builder.Services.AddSingleton(
    new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.EUCentral1));
builder.Services.AddScoped<IZeroApiRepository, ZeroApiRepository>();
builder.Services.AddSingleton<IZeroRealtimeRepository, ZeroRealtimeRepository>();

// Manager
builder.Services.AddSingleton<IOrderSignalDetectorManager, OrderSignalDetectorManager>();
builder.Services.AddSingleton<IOrderSignalManager, OrderSignalManager>();
builder.Services.AddSingleton<IInstrumentStorageManager, InstrumentStorageManager>();
builder.Services.AddSingleton<ITradingStorageManager, TradingStorageManager>();
builder.Services.AddSingleton<IRealtimeQuotesManager, RealtimeQuotesManager>();


builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapZeroEndpoints();
app.MapZeroRealtimeEndpoints();

app.Run();

