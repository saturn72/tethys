﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.DbModel.Repositories.LiteDb;
using Tethys.Server.Services;
using Tethys.Server.Hubs;

namespace Tethys.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var configFilePath = ConfigManager.GetConfigFile(configuration["config"] ?? "appsettings.json");

            var configDirectory = Path.GetDirectoryName(configFilePath);
            Configuration = new ConfigurationBuilder()
                .SetBasePath(configDirectory)
                .AddJsonFile(configFilePath)
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
            services.AddSignalR(options => options.EnableDetailedErrors = true);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Tethys API", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
                c.DescribeAllParametersInCamelCase();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            var dbName = Configuration["tethysConfig:liteDb:name"];
            services.AddTransient(sr => new UnitOfWorkLiteDb(dbName));
            services.AddTransient<IHttpCallRepository, HttpCallRepositoryLiteDb>();
            services.AddTransient<INotificationRepository, NotificationRepositoryLiteDb>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationPublisher, NotificationPublisher>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            var tethysConfig = TethysConfig.FromConfiguration(Configuration);

            var rewriteOptions = new RewriteOptions();
            rewriteOptions //.AddRewrite(@"^(?i)(?!)tethys/(.*)", "mock/$1", true)
                .Add(rCtx => RedirectRules.RedirectRequests(rCtx.HttpContext.Request, tethysConfig));
            app.UseRewriter(rewriteOptions);
            app.UseCors(cp => cp.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "tethys/swagger";
                c.SwaggerEndpoint(Consts.SwaggerEndPointPrefix + "/v1/swagger.json", "Tethys API");
            });
            app.UseSignalR(router => router.MapHub<MockHub>(Consts.TethysWebSocketPath));
            app.UseMvc();
        }
    }
}