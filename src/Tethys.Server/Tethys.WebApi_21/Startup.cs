using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.DbModel.Repositories.LiteDb;
using Tethys.WebApi.Hubs;

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
            services.AddSignalR(options => options.EnableDetailedErrors = true);

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
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            var rewriteOptions = new RewriteOptions();
            rewriteOptions//.AddRewrite(@"^(?i)(?!)tethys/(.*)", "mock/$1", true)
                .Add(RedirectRules.RedirectRequests);
            app.UseRewriter(rewriteOptions);

            app.UseCors(cp => cp.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "tethys/swagger";
                c.SwaggerEndpoint(Consts.SwaggerEndPointPrefix + "/v1/swagger.json", "Tethys API");
            });

            app.UseSignalR(router =>
            {
                router.MapHub<MockHub>("/ws");
            });
            app.UseMvc();
        }
    }
}