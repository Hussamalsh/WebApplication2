using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApplication2.Filters;
using WebApplication2.Infrastructure;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2
{
    public class Startup
    {
        private readonly int? _httpsPort;

        public Startup(IConfiguration configuration/*, IHostingEnvironment env*/)
        {
            Configuration = configuration;

            /*var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Get the HTTPS port (only in development)
            if (env.IsDevelopment())
            {
                var launchJsonConfig = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Properties\\launchSettings.json")
                    .Build();
                _httpsPort = launchJsonConfig.GetValue<int>("iisSettings:iisExpress:sslPort");
            }*/
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HotelApiContext>(opt => opt.UseInMemoryDatabase());
            services.AddAutoMapper();

            services.AddMvc( opt =>
                {
                    opt.Filters.Add(typeof(JsonExceptionFilter));
                    opt.Filters.Add(typeof(LinkRewritingFilter));
                    // we are adding ios json formatter
                    var jsonFormatter = opt.OutputFormatters.OfType<JsonOutputFormatter>().Single();
                    opt.OutputFormatters.Remove(jsonFormatter);
                    opt.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));
                })
                .AddJsonOptions(opt =>
                {
                    // These should be the defaults, but we can be explicit:
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    opt.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    opt.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new MediaTypeApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            });


            services.Configure<HotelOptions>(Configuration);
            services.Configure<HotelInfo>(Configuration.GetSection("Info"));
            services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));

            services.AddScoped<IRoomService, DefaultRoomService>();
            services.AddScoped<IOpeningService, DefaultOpeningService>();
            services.AddScoped<IBookingService, DefaultBookingService>();
            services.AddScoped<IDateLogicService, DefaultDateLogicService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
                                IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var context = serviceProvider.GetRequiredService<HotelApiContext>();
                var dateLogicService = serviceProvider.GetRequiredService<IDateLogicService>();//app.ApplicationServices.GetRequiredService<>();
                AddTestData(context, dateLogicService);
            }
            
            app.UseHsts(opt => 
            {
                opt.MaxAge(days: 180);
                opt.IncludeSubdomains();
                opt.Preload();
            });
            app.UseHttpsRedirection();
            app.UseMvc();
            //app.UseApiVersioning();
        }


        /*private static void AddTestData(HotelApiContext context)
        {
            context.Rooms.Add(new Models.RoomEntity
            {
                Id = Guid.Parse("f15fcda8-47ef-4c1e-b666-b1ada67384c2"),
                Name = "Oxford Suite",
                Rate = 10119
            });

            context.Rooms.Add(new Models.RoomEntity
            {
                Id = Guid.Parse("85cf6475-5e96-458f-914e-bf3f7049839c"),
                Name = "Driscoll Suite",
                Rate = 23959
            });

            context.SaveChanges();

        }*/

        private static void AddTestData(
            HotelApiContext context,
            IDateLogicService dateLogicService)
        {
            var oxford = context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("301df04d-8679-4b1b-ab92-0a586ae53d08"),
                Name = "Oxford Suite",
                Rate = 10119,
            }).Entity;

            context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195976"),
                Name = "Driscoll Suite",
                Rate = 23959
            });

            var today = DateTimeOffset.Now;
            var start = dateLogicService.AlignStartTime(today);
            var end = start.Add(dateLogicService.GetMinimumStay());

            context.Bookings.Add(new BookingEntity
            {
                Id = Guid.Parse("2eac8dea-2749-42b3-9d21-8eb2fc0fd6bd"),
                Room = oxford,
                CreatedAt = DateTimeOffset.UtcNow,
                StartAt = start,
                EndAt = end,
                Total = oxford.Rate,
            });
            context.SaveChanges();
        }
    }
}
