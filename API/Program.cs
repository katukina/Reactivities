using API.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//KP Before autorization use the one we have created
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

//KP Create DB Get access to DataContext service
//"using" should be added to automatically cleans up when finished with the code
using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;

//try to create DB
try
{
    var context = service.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync() ;
    await Seed.SeedData(context); //filling the created DB
}
catch (Exception ex)
{
   var logger = service.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occured during migration");
}

app.Run();
