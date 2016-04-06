using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoothubAPI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using YoothubAPI.Services;
using Serilog;
using System.IO;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.WebEncoders;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authentication.Cookies;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Localization;
using System.Globalization;
using System.Collections.Generic;

namespace YoothubAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<IdentityOptions>(opt =>
            {
                opt.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = 401;
                        return Task.FromResult(0);
                    }
                };
            });

            // build connection string
            var dbHostName = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? Configuration["Database:Host"];
            var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? Configuration["Database:Port"];
            var dbUsername = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? Configuration["Database:Username"];
            var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? Configuration["Database:Password"];
            var dbDatabase = Environment.GetEnvironmentVariable("POSTGRES_DATABASE") ?? Configuration["Database:Database"];
            var connectionString = GetConnectionString(dbHostName, dbPort, dbUsername, dbPassword, dbDatabase);

            // Add framework services.
            services.AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen();

            services.AddSingleton<IPlaybackService, PlaybackService>();

            services.AddTransient<IYoutubeService, YoutubeService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog(new LoggerConfiguration()
                .WriteTo.RollingFile("logs/log.log")
                .MinimumLevel.Verbose()
                .CreateLogger());

            // Configure the localization options
            app.UseRequestLocalization(new RequestCulture("pl-PL"));

            app.UseExceptionHandler(errorApp => 
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body>\r\n");
                    await context.Response.WriteAsync("We're sorry, we encountered an un-expected issue with your application.<br>\r\n");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        // This error would not normally be exposed to the client
                        await context.Response.WriteAsync("<br>Error: " + HtmlEncoder.Default.HtmlEncode(error.Error.ToString() + " " + error.Error.Message) + "<br>\r\n");
                    }
                    await context.Response.WriteAsync("<br><a href=\"/\">Home</a><br>\r\n");
                    await context.Response.WriteAsync("</body></html>\r\n");
                    await context.Response.WriteAsync(new string(' ', 512)); // Padding for IE
                });
            });

            app.UseIdentity();

            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = "183555151952-07kf9hqu1a2g7ihkifchqjotnh98ce9c.apps.googleusercontent.com";
                options.ClientSecret = "3mYjlHirja6kj-QjJDC57SFf";
                options.CallbackPath = new PathString("/api/signin-google");
            });

            app.UseMvc();

            app.UseSwaggerGen("api/swagger/{apiVersion}/swagger.json");
            app.UseSwaggerUi("api/docs", "/api/swagger/v1/swagger.json");

        }

        private string GetConnectionString(string host, string port, string username, string password, string database)
        {
            return $"Host={host};Port={port};Username={username};Password={password};Database = {database};";
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
