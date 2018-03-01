using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Consul;

namespace aspnet1
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
            this.RegisterConsul();
            services.AddMvc();
        }

        public WriteResult RegisterConsul()
        {
			var client = new ConsulClient();

			var name = Assembly.GetEntryAssembly().GetName().Name;
			var port = 5001;
			var id = $"{name}:{port}";

			var tcpCheck = new AgentServiceCheck()
			{
				DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
				Interval = TimeSpan.FromSeconds(30),
				TCP = $"127.0.0.1:{port}"
			};

			var httpCheck = new AgentServiceCheck()
			{
				DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
				Interval = TimeSpan.FromSeconds(30),
				HTTP = $"http://127.0.0.1:{port}/Ping"
			};

			var registration = new AgentServiceRegistration()
			{
				Checks = new[] { tcpCheck, httpCheck },
				Address = "127.0.0.1",
				ID = id,
				Name = name,
				Port = port
			};

			return client.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
