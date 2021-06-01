using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupervisorAPI.Infrastructure.AzureStorageSetting;
using SupervisorAPI.Service.BusinessLogic;
using SupervisorAPI.Service.Contract;

namespace SupervisorAPI
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
            services.AddControllers();
            services.Configure<AzureStorageConnection>(Configuration.GetSection("Data:Azure"));
            QueueCreator.CreateAzureQueues(Configuration["Data:Azure:ConnectionString"], "orderqueue");
            services.AddTransient<IOrderQueue, OrderQueue>();
            TableCreator.CreateAzureTables(Configuration["Data:Azure:ConnectionString"], "confirmation");
            services.AddTransient<IConfirmationTable, ConfirmationTable>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
