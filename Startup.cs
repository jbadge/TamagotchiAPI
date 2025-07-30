using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TamagotchiAPI.Configuration;
using TamagotchiAPI.Models;

namespace TamagotchiAPI
{
    public class Startup
    {
        // When the system starts up, we will be given a configuration variable.
        // we will save this in a property named `Configuration`
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Where we store our program's configuration
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Use NewtonsoftJson to avoid JSON cyclical loops
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // Add support for CORS which allow cross domain access to the API
            services.AddCors();

            // Configure the Swagger documentation engine to generate documentation for our API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TamagotchiAPI", Version = "v1" });
            });

            // Add IHttpContextAccessor to access HTTP context in controllers and middleware
            services.AddHttpContextAccessor();

            // Configure the class to use for a DatabaseContext
            services.AddDbContext<DatabaseContext>();

            // Bind configuration section "AppSettings" to AppSettings class
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // If we are in development
            if (env.IsDevelopment())
            {
                // Use a friendly error page that helps the developer.
                // We wouldn't want this in production since it might
                // give away code secrets.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Only enforce https in production
                // app.UseHttpsRedirection();

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure CORS to allow access from everywhere
            app.UseCors(builder =>
              builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin()
               );

            // Indicate we are using the Swagger documentation system
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TamagotchiAPI");
                c.RoutePrefix = String.Empty;
            });

            // Use custom middleware to enforce presence of 'x-visitor-id' header
            // and set visitor ID in PostgreSQL session variable for RLS
            app.UseMiddleware<VisitorIdMiddleware>();

            // Middleware to inject Visitor ID into PostgreSQL session variable
            // Enable Row-Level Security policies to filter data per visitor
            app.Use(async (context, next) =>
                {
                    var visitorId = context.Request.Headers["x-visitor-id"].FirstOrDefault();
                    var adminVisitorId = Configuration["AdminVisitorId"];

                    if (!string.IsNullOrEmpty(visitorId))
                    {
                        // var db = context.RequestServices.GetRequiredService<DatabaseContext>();

                        var db = context.RequestServices.GetRequiredService<DatabaseContext>();

                        // Make sure connection is open before setting role
                        if (db.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        {
                            await db.Database.OpenConnectionAsync();
                        }

                        if (visitorId == adminVisitorId)
                        {
                            // Set admin role — no claim needed
                            await db.Database.ExecuteSqlRawAsync("set local role admin_role");
                        }
                        else
                        {
                            // Set visitor role and claim
                            await db.Database.ExecuteSqlRawAsync("set local role visitor_role");

                            // Escape single quotes in visitorId to avoid SQL injection
                            var escaped = visitorId.Replace("'", "''");
                            var sql = "set local request.jwt.claim.sub = '" + escaped + "'";
                            await db.Database.ExecuteSqlRawAsync(sql);
                        }
                    }

                    await next();

                    // var db = context.RequestServices.GetRequiredService<DatabaseContext>();
                    // await db.Database.CloseConnectionAsync();
                });

            // Use routing to determine which endpoints are handled by which controllers and methods
            app.UseRouting();

            // Enable the use of user authorization if we want to use that.
            app.UseAuthorization();

            // Hook up our endpoints (URLs) to the controllers and methods that handle them.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
