using Amazon.CognitoIdentityProvider;
using TradingApi.Repositories.ZeroApi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using TradingApi.Authentication;
using System.Security.Claims;
using Neo4j.Driver;
using TradingApi.Repositories.ZeroRealtime;
using TradingApi.Endpoints.ZeroApi;
using TradingApi.Endpoints.ZeroRealtime;
using TradingApi.Manager.RealtimeQuotesStorage;
using MediatR.NotificationPublishers;
using TradingApi.Manager.OrderSignalDetector;
using TradingApi.Manager.OrderSignalDetector.Detectors;
using System.Reflection;

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

builder.Services.AddSingleton(
    new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.EUCentral1));

builder.Services.AddScoped<IZeroApiRepository, ZeroApiRepository>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ZeroTradeAuthenticationHandler>("ZeroTrade", null);
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ZeroTrade", policy => policy.RequireAuthenticatedUser()
    .RequireClaim(ClaimTypes.Name, "ZeroTrade"));
});

builder.Services.AddSingleton(
    GraphDatabase.Driver(
        builder.Configuration["NEO4J_SERVER"],
        AuthTokens.Basic(builder.Configuration["NEO4J_USER"], builder.Configuration["NEO4J_PASSWORD"])
    )
);


builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.NotificationPublisher = new TaskWhenAllPublisher();
    cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
    cfg.Lifetime = ServiceLifetime.Singleton;
});

builder.Services.AddSingleton<IZeroRealtimeRepository, ZeroRealtimeRepository>();
builder.Services.AddSingleton<IRealtimeQuotesStorageManager, RealtimeQuotesStorageManager>();

// find all classes with interface of IOrderSignalDetector and register as singleton with DepedencyInject without abstract classes
Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IOrderSignalDetector)))
    .ToList().ForEach(t => builder.Services.AddSingleton(typeof(IOrderSignalDetector), t));
builder.Services.AddSingleton<IOrderSignalDetectorManager, OrderSignalDetectorManager>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//var todosApi = app.MapGroup("/todos");
//todosApi.MapGet("/", () => sampleTodos);
//todosApi.MapGet("/{id}", (int id) =>
//    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

app.UseAuthentication();
app.UseAuthorization();

app.MapZeroEndpoints();
app.MapZeroRealtimeEndpoints();

await app.Services.GetRequiredService<IOrderSignalDetectorManager>()
    .StartAsync();

app.Run();

