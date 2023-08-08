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

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (env != "Development") {
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(System.Net.IPAddress.Any, 8080);
    });
}

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

app.UseXContentTypeOptions(); // prevent mime sniffing of the content type
app.UseReferrerPolicy(opt => opt.NoReferrer());
app.UseXXssProtection(opt => opt.EnabledWithBlockMode()); // add a cross-site scripting protection header
app.UseXfo(opt => opt.Deny()); //Prevent our app being used inside an iframe whic proctects against that click jacking
// app.UseCspReportOnly can be used first to check the problems when inspect
//Next one main defense against cross-site scripting attacks
app.UseCsp(opt => opt
    .BlockAllMixedContent() //force our app to load only HTTP content
    //All of this content is allowed to be served from what we are running
    .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com"))
    .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:")) //data for the domain that console was was showing
    .FormActions(s => s.Self())
    .FrameAncestors(s => s.Self())
    .ImageSources(s => s.Self().CustomSources("blob:", "https://res.cloudinary.com"))
    .ScriptSources(s => s.Self())
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else 
{
    app.Use(async (context, next) => 
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        await next.Invoke();
    });
}

//KP Before autorization use the one we have created
app.UseCors("CorsPolicy");
//Identity
app.UseAuthentication();
app.UseAuthorization();

//After building the client end and wwwroot folder is created with the needed content
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
//New hub and the route that we are going to direct user to when they connet to our chat hub
app.MapHub<ChatHub>("/chat");
//From API\Controllers\FallbackController.cs Firt parameter action Index inside that file, second the controller
app.MapFallbackToController("Index", "Fallback");

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
