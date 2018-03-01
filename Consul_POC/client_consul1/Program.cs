using System;
using System.Reflection;
using System.Threading.Tasks;
using Consul;
using System.Linq;
using System.Collections.Generic;

namespace client_consul1
{
    class Program
    {
		

        static void Main(string[] args)
        {
            CheckServices().Wait();
            System.Console.Write("OK");
        }

        public static async Task CheckServices()
        {
			var _consulClient = new ConsulClient(c =>
			{
				var uri = new Uri("http://127.0.0.1:8500");
				c.Address = uri;
			});

            try
            {
                var services = await _consulClient.Agent.Services();
				foreach (var service in services.Response)
				{
                    System.Console.WriteLine(service.Value.Service);
                    System.Console.WriteLine(service.Value.ID);
                    System.Console.WriteLine(service.Value.Address);
				}
            }
            catch (Exception ex)
            {
                System.Console.Write(ex.Message);
            }

        }
        /*
		private static async Task CheckServerHealth()
		{
			var _consulClient = new ConsulClient(c =>
			{
				var uri = new Uri("http://127.0.0.1:8500");
				c.Address = uri;
			});
			var checks = await _consulClient.Health.Service("aspnet1");
			foreach (var entry in checks.Response)
			{
				var check = entry.Checks.SingleOrDefault(c => c.ServiceName == "aspnet1");
				if (check == null) continue;
				var isPassing = check.Status == HealthStatus.Passing;
				var serviceUri = new Uri($"{entry.Service.Address}:{entry.Service.Port}");
				if (isPassing)
				{
                    //Add to service list
                    System.Console.WriteLine("Service local online.");
				}
				else
				{
					//Add to service list
					System.Console.WriteLine("Service local offline.");
				}
			}
		}
		*/
    }
}
