using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DativeBackend.Models;

namespace DativeBackend {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var key = Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SecretKey"));
            
            // MySQL connection configuration (as defined at appsettings)
            // TODO Set password as secret (see: https://docs.microsoft.com/es-es/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux)
            services.AddDbContext<CustomerContext>(options => 
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            
            // JWT authentication configuration
            // TODO Read Auth0 good practices (see: https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/)
            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>  {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            // Redis cache configuration
            // TODO Check ConnectionMultiplexer as alternative
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = Configuration.GetConnectionString("RedisCacheConnection");
            });

            services.AddControllers();

            // Register the Swagger generator
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
