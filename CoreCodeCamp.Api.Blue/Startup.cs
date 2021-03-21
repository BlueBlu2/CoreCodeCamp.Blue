using CoreCodeCamp.Api.Blue.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using CoreCodeCamp.Api.Blue.Controllers;

namespace CoreCodeCamp.Api.Blue
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
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                //config.ApiVersionReader = new UrlSegmentApiVersionReader();
                //config.ApiVersionReader = new QueryStringApiVersionReader("ver");
                //config.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                //config.ApiVersionReader = ApiVersionReader.Combine( new HeaderApiVersionReader("X-Version"), new QueryStringApiVersionReader("ver","version"));
                //config.Conventions.Controller<TalksController>().HasApiVersion(1, 0).HasApiVersion(1, 1).Action(c => c.Delete(default(string), default(int))).MapToApiVersion(1, 1);
            });
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
