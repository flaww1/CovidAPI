using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CovidAPI.Models;
using CovidAPI.Services;
using CovidAPI.Services.Rest;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CovidAPI
{

   /// <summary>
   /// Configures the application services and requests during startup.
   /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration settings.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration settings.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services used by the application.
        /// </summary>
        /// <param name="services">The service collection.</param>

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped<ICovidDataService, CovidDataService>();
            services.AddHttpClient();

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CovidAPI", Version = "v1", Description = "Covid-19 Testing Data API with Geolocation Integration",
                    Contact = new OpenApiContact
                    {
                        Name = "Flávio Pereira",
                        Email = "a21110@alunos.ipca.pt",
                    },
                });
                var appEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
                var xmlPath = Path.Combine(appEnvironment == Environments.Development ? AppContext.BaseDirectory : "app", xmlFile);

                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            });

       
            services.AddScoped<IGeolocationService, GeolocationService>();
            services.AddSingleton<GeolocationCache>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "SampleInstance";
            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"] ?? "default_issuer",
                    ValidAudience = Configuration["Jwt:Audience"] ?? "default_audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"] ?? "default_secret"))

                };
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAuthService, AuthService>();
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));


        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
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
            app.UseCors("AllowAll");

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(new ErrorResponse { Message = "Internal Server Error" }.ToString());
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }


        /// <summary>
        /// Configures logging services for the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
        }
    }
}
