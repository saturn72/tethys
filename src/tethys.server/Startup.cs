using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Tethys.Server.DbModel;
using Tethys.Server.Services;
using Tethys.Server.Services.Notifications;
using Tethys.Server.Hubs;
using Tethys.Server.Middlewares;
using Microsoft.Extensions.FileProviders;
using Tethys.Server.Swagger;
using Tethys.Server.Services.HttpCalls;
using LiteDB;
using Tethys.Server.Models;

namespace Tethys.Server
{
    public class Startup
    {
        #region consts
        private const string CorsPolicy = "CorsPolicy";
        #endregion
        private static readonly string _staticHtmlFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "UI");
        private static bool _hasStaticFiles = Directory.Exists(_staticHtmlFilesPath);
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
        public TethysConfig TethysConfig { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(options => options.AddPolicy(CorsPolicy, builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials();
        }));
            services.AddSignalR(options => options.EnableDetailedErrors = true);

            TethysConfig = TethysConfig.FromConfiguration(Configuration);
            services.AddSingleton<TethysConfig>(sp => TethysConfig);

            services.AddSingleton<TethysHub>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Tethys API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Name = "Click for dashboard",
                        Url = _hasStaticFiles ? "./../ui/index.html" : "Not html files found. Verify you run tethys from it's base directory"
                    },
                });
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<FileUploadOperation>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            AddLiteDb(services, Configuration);
            services.AddTransient<IHttpCallService, HttpCallService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IFileUploadManager, FileUploadManager>();

            services.AddSingleton<IRequestResponseCoupleService, RequestResponseCoupleService>();
        }

        private void AddLiteDb(IServiceCollection services, IConfiguration configuration)
        {
            var dbName = Configuration["tethysConfig:liteDb:name"];

            using (var db = new LiteDatabase(dbName))
            {
                db.GetCollection<HttpCall>().EnsureIndex("Request.Resource");
                db.GetCollection<HttpCall>().Delete(q => true);

                db.GetCollection<PushNotification>().EnsureIndex("Key");
                db.GetCollection<RequestResponseCouple>().EnsureIndex("Request.Resource");
            }
            services.AddTransient<IRepository<HttpCall>>(sr => new LiteDbRepository<HttpCall>(dbName));
            services.AddTransient<IRepository<RequestResponseCouple>>(sr => new LiteDbRepository<RequestResponseCouple>(dbName));
            services.AddTransient<IRepository<PushNotification>>(sr => new LiteDbRepository<PushNotification>(dbName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            //log incoming and outgoing traffic

            app.UseMiddleware<RequestResponseLoggingMiddleware>();


            if (_hasStaticFiles)
            {
                var staticFileOptions = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(_staticHtmlFilesPath),
                    RequestPath = Consts.ApiBaseUrl + Consts.Ui
                };
                app.UseStaticFiles(staticFileOptions);
            }
            else
            {
                var tmpColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{_staticHtmlFilesPath} could not be found - static html content is not rendered");
                Console.ForegroundColor = tmpColor;
            }


            var rewriteOptions = new RewriteOptions();
            rewriteOptions //.AddRewrite(@"^(?i)(?!)tethys/(.*)", "mock/$1", true)
                .Add(rCtx => RedirectRules.RedirectRequests(rCtx.HttpContext.Request, TethysConfig));
            app.UseRewriter(rewriteOptions);
            app.UseCors(CorsPolicy);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "tethys/swagger";
                c.SwaggerEndpoint(Consts.SwaggerEndPointPrefix + "/v1/swagger.json", "Tethys API");
            });

            app.UseSignalR(router =>
            {
                router.MapHub<TethysHub>(Consts.TethysWebSocketPath);
                //TODO: this is not eanough - clients are registered to the same group and would get un wanted notifications.
                foreach (var wss in TethysConfig.WebSocketSuffix)
                    router.MapHub<MockHub>(wss);
            });
            app.UseMvc();
        }
    }
}
