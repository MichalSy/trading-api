using Amazon.CognitoIdentityProvider;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using TradingApi;
using TradingApi.Authentication;
using TradingApi.Endpoints.ZeroApi;
using TradingApi.Endpoints.ZeroRealtime;
using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignalDetector;
using TradingApi.Manager.OrderSignalDetector.Detectors;
using TradingApi.Manager.RealtimeQuotesStorage;
using TradingApi.Manager.TradingStorage;
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



// Intern Communication
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.NotificationPublisher = new TaskWhenAllPublisher();
    cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
    cfg.Lifetime = ServiceLifetime.Singleton;
});



// Repository
builder.Services.AddSingleton(
    new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.EUCentral1));
builder.Services.AddScoped<IZeroApiRepository, ZeroApiRepository>();
builder.Services.AddSingleton<IZeroRealtimeRepository, ZeroRealtimeRepository>();


builder.Services.AddMongoDb();


// Manager
builder.Services.AddSingleton<IRealtimeQuotesStorageManager, RealtimeQuotesStorageManager>();

// find all classes with interface of IOrderSignalDetector and register as singleton with DepedencyInject without abstract classes
Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IOrderSignalDetector)))
    .ToList().ForEach(t => builder.Services.AddSingleton(typeof(IOrderSignalDetector), t));
builder.Services.AddSingleton<IOrderSignalDetectorManager, OrderSignalDetectorManager>();
builder.Services.AddSingleton<IOrderSignalManager, OrderSignalManager>();
builder.Services.AddSingleton<ITradingStorageManager, TradingStorageManager>();


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

