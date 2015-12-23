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


            // Add framework services.
            services.AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<ApplicationDbContext>();

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
            TextWriter writer = new StreamWriter(new FileStream("log.log", FileMode.OpenOrCreate));
            loggerFactory.AddSerilog(new LoggerConfiguration()
                .WriteTo.TextWriter(writer)
                .MinimumLevel.Error()
                .CreateLogger());

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

                app.UseIdentity();

            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = "183555151952-kf6rvmkdtt9fq7kmnesq0tvgjv5dtjrc.apps.googleusercontent.com";
                options.ClientSecret = "Nx9QgB0A6QuhetihuH-7qBTg";
            });

            app.UseMvc();

            app.UseSwaggerGen();
            app.UseSwaggerUi("api/docs");

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
