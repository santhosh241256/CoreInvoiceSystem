using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IDatastore, InMemoryDataStore>();
builder.Services.AddTransient<IInvoiceService, InvoiceService>();   

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreInvoiceSystem API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreInvoiceSystem API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var datastore = app.Services.GetRequiredService<IDatastore>() as InMemoryDataStore;
datastore?.InitializeSampleData();

app.Run();
