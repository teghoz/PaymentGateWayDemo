using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using MerchantDbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using PaymentGateway.Validators;
using PaymentGatewayDbContext;
using SharedResource.ViewModels;
using System.Text;
using Microsoft.OpenApi.Models;
using PaymentGateway.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.IO;
using Hangfire;
using Hangfire.Console;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "checkOutAllowedOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddFluentValidation(
                fv => {
                    //fv.RegisterValidatorsFromAssemblyContaining<PaymentValidator>();
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
            //services.AddTransient<IValidator<PaymentInfo>, PaymentValidator>();

            services.AddDbContext<PaymentGatewayDbContext.PaymentGatewayDbContext>(options =>
                options.UseSqlServer(
                    ConnectionManager.Connection["ConnectionString:PaymentGateway"]).EnableSensitiveDataLogging())
                    .AddIdentity<PaymentGatewayDbContext.ApplicationUser, IdentityRole>().AddEntityFrameworkStores<PaymentGatewayDbContext.PaymentGatewayDbContext>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<PaymentGatewayDbContext.PaymentGatewayDbContext>();


            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .SetIsOriginAllowed(c => c.Contains(":3000"))
                    //.AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));
                });
            });

            var appSettingsSection = Configuration.GetSection("ApplicationSettings");
            services.Configure<ApplicationSettings>(appSettingsSection);
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<ApplicationSettings>();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Checkout Payment Gateway API",
                    Description = "CheckOut Payment Gateway API",
                    TermsOfService = new Uri("https://www.CheckOut.com/termsandconditions"),
                    Contact = new OpenApiContact
                    {
                        Name = "Checkout Ltd",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/@CheckOut"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                var xmlFile = $".xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddLogging();
            services.AddHangfire(config => config.UseSqlServerStorage(ConnectionManager.Connection["ConnectionString: PaymentGateway"]));
            services.AddHangfire(config => {
                config.UseSqlServerStorage(ConnectionManager.Connection["ConnectionString"]);
                config.UseConsole();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
                c.RoutePrefix = string.Empty;
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //IHost not registering --IHost host //need to investigate
            //MerchantSeeds.MerchantSeedAsync(host).Wait();
            //PaymentGatewaySeeds.PaymentGatewaySeedAsync(host).Wait();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //since it is a demo i am leaving out the authorization
                //Authorization = new[] { new CheckoutHangfireAuthorizationFilter() },
                DashboardTitle = "Checkout Job Manager"
            });

            var options = new BackgroundJobServerOptions
            {
                WorkerCount = Environment.ProcessorCount * 5,
                ShutdownTimeout = TimeSpan.FromSeconds(150)
            };
            app.UseHangfireServer(options);
        }
    }
}
