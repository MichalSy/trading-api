using TradingApi;
using Amazon.CognitoIdentityProvider;
using TradingApi.Repositories.Zero;
using Microsoft.AspNetCore.OpenApi;
using TradingApi.Endpoints.Zero;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using TradingApi.Authentication;
using System.Security.Claims;
using Amazon.Runtime;

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

builder.Services.AddScoped<IZeroRepository, ZeroRepository>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ZeroTradeAuthenticationHandler>("ZeroTrade", null);
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ZeroTrade", policy => policy.RequireAuthenticatedUser()
    .RequireClaim(ClaimTypes.Name, "ZeroTrade"));
});


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

app.Run();

