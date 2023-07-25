using Application.Activities;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            //KP This was added manually after added DataContext class in Persistence
            services.AddDbContext<DataContext>(opt => {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });  //in appsettings.Development.json we need to add that connection
            //KP Srvice needed to add whcih contains Cors policy to let me access the api from react
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    //KP Alow any http method
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000"); //This has to match where the request is coming, our React app
                });
            });
            //KP SErive registered and need to find out wher are our handlers because we are adding this service in API project
            services.AddMediatR(typeof(List.Handler));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            //This will make this available to be injected inside our application handlers
            services.AddScoped<IUserAccessor, UserAccessor>();
            return services;
        }        
    }
}