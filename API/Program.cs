using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//KP This was added manually after added DataContext class in Persistence
builder.Services.AddDbContext<DataContext>(opt => {
   opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//KP Srvice needed to add whcih contains Cors policy to let me access the api from react
builder.Services.AddCors(opt => {
    opt.AddPolicy("CorsPolicy", policy => {
        //KP Alow any http method
        policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000"); //This has to match where the request is coming, our React app
    });
});

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
