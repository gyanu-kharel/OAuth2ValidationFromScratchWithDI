using System;
using Microsoft.Extensions.DependencyInjection;

namespace TestingToken
{
    public class Program 
    {
        private static IServiceProvider _serviceProvider;
       
        static void Main(string[] args)
        {

            RegisterServices();
            //IServiceScope scope = _serviceProvider.CreateScope();
            //scope.ServiceProvider.GetRequiredService<ConsoleApplication>().Run();
            var consoleApp = _serviceProvider.GetService<ConsoleApplication>();
            consoleApp.Run();
            DisposeServices();            
        }

        private static void RegisterServices()
        {
            // Registering services for dependency injection

            var services = new ServiceCollection();
            services.AddSingleton<ICore, Core>();
            services.AddSingleton<ConsoleApplication>();
            _serviceProvider = services.BuildServiceProvider();
            
        }

        private static void DisposeServices()
        {
            // Disposing services 

            if(_serviceProvider == null)
            {
                return;
            }
            if(_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
