using API.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => 
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//KP Before autorization use the one we have created
app.UseCors("CorsPolicy");
//Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//New hub and the route that we are going to direct user to when they connet to our chat hub
app.MapHub<ChatHub>("/chat");

//KP Create DB Get access to DataContext service
//"using" should be added to automatically cleans up when finished with the code
using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;

//try to create DB
try
{
    var context = service.GetRequiredService<DataContext>();
    var userManager = service.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync() ;
    await Seed.SeedData(context, userManager); //filling the created DB
}
catch (Exception ex)
{
   var logger = service.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occured during migration");
}

app.Run();
