using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.DbModel.Repositories.LiteDb;
using Tethys.WebApi.WebSockets;

namespace Tethys.WebApi
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
            services.AddMvc();
            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Tethys API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });
            var dbName = Configuration["liteDb:name"];
            services.AddTransient(sr => new UnitOfWorkLiteDb(dbName));
            services.AddTransient<IHttpCallRepository, HttpCallRepositoryLiteDb>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            var rewriteOptions = new RewriteOptions();
            rewriteOptions//.AddRewrite(@"^(?i)(?!)tethys/(.*)", "mock/$1", true)
                .Add(RedirectRules.RedirectRequests);

            ConfigureWebSockets(app);
            //redirect all incoming requests
            app.UseRewriter(rewriteOptions);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "tethys/swagger";
                c.SwaggerEndpoint(Consts.SwaggerEndPointPrefix + "/v1/swagger.json", "Tethys API");
            });

            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseMvc();
        }

        private void ConfigureWebSockets(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(1),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseCors(builder =>
                builder.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/tethys/ws")
                    await ManageWebSocketConnection();
                else
                    await next();
                async Task ManageWebSocketConnection()
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await WebSocketOutlet.Send(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
            });
        }
    }
}