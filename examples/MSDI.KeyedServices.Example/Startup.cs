using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDI.KeyedServices.Example.Services;
using System.Linq;

namespace MSDI.KeyedServices.Example
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
            // Register different greeters for supported languages

            services.AddKeyedService<IGreeter, EnglishGreeter, Language>(Language.En, sc =>
            {
                // Remember to register your implementation type without assigning it to service type                    
                sc.AddTransient<EnglishGreeter>();
            });

            // You can also register your implementation type seperetly
            services.AddKeyedService<IGreeter, PolishGreeter, Language>(Language.Pl, registration: null);
            services.AddTransient<PolishGreeter>();

            // That's it!
            // Now go to browser and request http://localhost:59792/api/hello?lang=pl

            // Rest of default registrations...
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var myServices = services.Where(x => x.ServiceType.Namespace.StartsWith("MSDI")).ToList();

            myServices.Count();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
