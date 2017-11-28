using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using api.core.Utils;
using api.core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api.core.Providers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace api.core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
                });
            });

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(new ExceptionFilter());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserManagement", policy => policy.RequireClaim("manage_user"));
                options.AddPolicy("Admin", policy => policy.RequireClaim("admin"));
                options.AddPolicy("User", policy => policy.RequireClaim("user"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApiDbContext>()
            .AddDefaultTokenProviders();

            // return 401 instead of redirect to login
            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToLogin = context => {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            var efConnection = Configuration["DefaultConnection"];
            services.AddDbContext<ApiDbContext>(options => options.UseSqlServer(efConnection));

            services.AddAuthentication(sharedOptions =>
                        {
                            sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                      .AddJwtBearer(cfg =>
                      {
                          cfg.RequireHttpsMetadata = false;
                          //cfg.SaveToken = true;
                          
                          cfg.TokenValidationParameters = new TokenValidationParameters()
                          {
                              ValidateIssuer = true,
                              ValidateAudience = true,
                              ValidateLifetime = true,
                              ValidateIssuerSigningKey = true,
                              ValidIssuer = Configuration["TokenAuthentication:Issuer"],
                              ValidAudience = Configuration["TokenAuthentication:Audience"],
                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenAuthentication:SecretKey"]))                              
                          };

                          cfg.Events = new JwtBearerEvents
                          {
                              OnAuthenticationFailed = context =>
                              {
                                  Console.WriteLine("OnAuthenticationFailed: " +
                                      context.Exception.Message);
                                  return Task.CompletedTask;
                              },
                              OnTokenValidated = context =>
                              {
                                  Console.WriteLine("OnTokenValidated: " +
                                      context.SecurityToken);
                                  return Task.CompletedTask;
                              }
                          };

                      });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // NOTE: DI is done here
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            ApiDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            Logging.InitLog4Net();
            Logging.GetLogger("core.api").Info("core.api starting");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});
            
            app.UseMiddleware<TokenProviderMiddleware>();
            app.UseMiddleware<RefreshTokenProviderMiddleware>();
            app.UseAuthentication();

            app.UseMvc();
        }
             
    }
}
