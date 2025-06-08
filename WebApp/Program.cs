using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Mapping;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");
Directory.CreateDirectory(keysFolder);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("ProductReviewAnalyzer.WebApp");

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<UserSession>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var cfg = TypeAdapterConfig.GlobalSettings;
cfg.Apply(new MappingProfile());
builder.Services.AddSingleton(cfg);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddRefitClient<IApiGatewayClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ApiGateway:BaseUrl"]!));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
