using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TamagotchiAPI.Models;

namespace TamagotchiAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TamagotchiAPI", Version = "v1" });
            });
            services.AddHttpContextAccessor();
            services.AddDbContext<DatabaseContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            app.UseCors(builder =>
              builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin()
               );
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TamagotchiAPI");
                c.RoutePrefix = String.Empty;
            });
            app.UseMiddleware<VisitorIdMiddleware>();
            app.Use(async (context, next) =>
                {
                    var visitorId = context.Request.Headers["x-visitor-id"].FirstOrDefault();
                    var adminVisitorId = Configuration["AdminVisitorId"];

                    if (!string.IsNullOrEmpty(visitorId))
                    {
                        var db = context.RequestServices.GetRequiredService<DatabaseContext>();

                        if (db.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        {
                            await db.Database.OpenConnectionAsync();
                        }

                        if (visitorId == adminVisitorId)
                        {
                            await db.Database.ExecuteSqlRawAsync("set role admin_role");
                        }
                        else
                        {
                            await db.Database.ExecuteSqlRawAsync("set role visitor_role");
                            var escaped = visitorId.Replace("'", "''");
                            var sql = "set local request.jwt.claim.sub = '" + escaped + "'";
                            await db.Database.ExecuteSqlRawAsync(sql);
                        }
                    }

                    await next();
                });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
