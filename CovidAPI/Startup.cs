using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using CovidAPI.Models;
using CovidAPI.Services;
using CovidAPI.Services.Rest;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace CovidAPI
{
    public class Startup
    {
        // Inject IConfiguration to access appsettings.json
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure DbContext with MySQL
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Add your services here
            services.AddScoped<ICovidDataService, CovidDataService>();

            services.AddHttpClient();

            // Add controllers and configure CORS policies
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Add Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CovidAPI", Version = "v1" });
            });
            services.AddScoped<IGeolocationService, GeolocationService>();
            services.AddScoped<GeolocationCache>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CovidAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll"); // Add this line

            // Enable CORS based on the configured policy
            app.UseCors("AllowAnyOriginMethodHeader");

            // Global Exception Handling Middleware
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    // Log the exception or perform other actions
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;
                    // Log or handle the exception as needed

                    // Return a JSON response with the error message
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500; // Internal Server Error
                    await context.Response.WriteAsync(new ErrorResponse { Message = "Internal Server Error" }.ToString());
                });
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole(); 
            });
        }


    }
}
