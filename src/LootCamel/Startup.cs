﻿using LootCamel.Data;
using LootCamel.Interfaces;
using LootCamel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LootCamel
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            // Add data contexts and repos
            services.AddDbContext<LootCamelContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ILootCamelRepository, LootCamelRepository>();

            // Here we add TelegramBotOption values and map them to relative class
            services.Configure<TelegramBotOptions>(Configuration.GetSection("TelegramBot"));
            services.Configure<LootBotOptions>(Configuration.GetSection("LootBot"));
            services.Configure<FellowAccessOptions>(Configuration.GetSection("FellowAccess"));

            // Add application services.
            services.AddTransient<IBotConnector, TelegramBotService>();
            services.AddTransient<IUserRegistrar, UserManagementService>();
            services.AddTransient<ILootShopExplorer, LootBotServices>();
            services.AddTransient<ILootItemsSource, LootBotServices>();
            services.AddTransient<IPriceNotifier, PriceNotificationService>();
            services.AddTransient<IPriceProvider, PriceHistoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LootCamelContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc(
                routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=ComingSoon}");
            }
            );

            DbInitializer.Initialize(context);
        }
    }
}
