using Amazon.CognitoIdentityProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using TradingApi;
using TradingApi.Authentication;
using TradingApi.Converters;
using TradingApi.Endpoints.SignalDetector;
using TradingApi.Endpoints.ZeroApi;
using TradingApi.Endpoints.ZeroRealtime;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Manager.Storage.OrderSignal;
using TradingApi.Manager.Storage.SignalDetector;
using TradingApi.Manager.Storage.SignalDetector.Detectors;
using TradingApi.Manager.Storage.TradingStorage;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.Storages;
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

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
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

// Storages
builder.Services.AddSingleton<IInstrumentStorage, InstrumentStorage>();
builder.Services.AddSingleton<ISignalDetectorStorage, SignalDetectorStorage>();
builder.Services.AddSingleton<IOrderSignalStorage, OrderSignalStorage>();

// Manager
builder.Services.AddSingleton<ISignalDetectorManager, SignalDetectorManager>();
builder.Services.AddSingleton<IOrderSignalManager, OrderSignalManager>();
builder.Services.AddSingleton<ITradingStorageManager, TradingStorageManager>();
builder.Services.AddSingleton<IRealtimeQuotesManager, RealtimeQuotesManager>();


// Detectors
// find all classes with interface of IOrderSignalDetector and register as singleton with DepedencyInject without abstract classes
Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IOrderSignalDetector)))
    .ToList().ForEach(t => builder.Services.AddSingleton(typeof(IOrderSignalDetector), t));

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
app.MapSignalDetectorEndpoints();

app.Run();

