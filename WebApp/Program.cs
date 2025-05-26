using Mapster;
using MapsterMapper;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Mapping;
using System.Reflection;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Apply(new MappingProfile());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Http Clients (API Gateway REST)
builder.Services.AddRefitClient<IApiGatewayClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ApiGateway:BaseUrl"]!));

// UI Services
//builder.Services.AddScoped<IApiGatewayClient, ApiGatewayClient>();

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();