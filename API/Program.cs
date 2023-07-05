using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//This was added manually after added DataContext class in Persistence
builder.Services.AddDbContext<DataContext>(opt => {
   opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

//Create DB Get access to DataContext service
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
