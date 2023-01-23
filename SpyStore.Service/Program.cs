using System.IO;
using System.Reflection;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Inicialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.Repos;
using SpyStore.DAL.Repos.Interfaces;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using System.Configuration;
using SpyStore.Service.Filters;

var builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment _env = builder.Environment;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().ConfigureApiBehaviorOptions(options => {
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressInferBindingSourcesForParameters = true;
}) ;
builder.Services.AddMvcCore(
    config => config.Filters.Add(new SpyStoreExceptionFIlter(_env))
    );

// configuracion del formato de json
builder.Services.Configure<JsonOptions>(options =>
{

    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});


// configurar cors, con esta configuracion permite todo, hay que personalizar
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            ;
        });
    }
    );

// agregar el contexto de entitiframework
var connstring = builder.Configuration.GetConnectionString("SpyStore");
builder.Services.AddDbContextPool<StoreContext>( options => options.UseSqlServer(connstring)) ;

builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IShoppingCartRepo, ShoppingCartRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();

builder.Services.AddSwaggerGen(
    c=>
    {
        c.SwaggerDoc(
            "v1",
            new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title ="SpyStore Service",
                Version = "v1",
                Description ="Service to support the SpyStore sample eCommerce site",
                TermsOfService = new Uri("http://localhost:23741/TermsOfService.txt"),
                License  = new Microsoft.OpenApi.Models.OpenApiLicense
                {
                    Name = "Freeware",
                    Url = new Uri("http://localhost:23741/LICENSE.txt")
                }
            } );
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

    } );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpyStore Service");
        }) ;


    // en el libro app.ApplicationServices se reemplaza por app.services
    using (var serviceScope = app
                    .Services
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<StoreContext>();
        //SampleDataInitializer.InitializeData(context);
    }

}
app.UseStaticFiles();
app.UseCors("AllowAll");




var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
