using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugLauncher
{
    using Microsoft.Owin.Hosting;

    using Ticketmaster.Dsc.DscManager;

    class Program
    {
        static void Main(string[] args)
        {
            var rOptions= new StartOptions();
            rOptions.Urls.Add("http://+:81");
            var host = WebApp.Start<Ticketmaster.Dsc.WebRepository.OwinStartup>(rOptions);
            var options = new StartOptions();
            options.Urls.Add("http://+:80/");
            var dscOptions = Ticketmaster.Dsc.DscManager.OwinStartup.GetDscManagerOptions();
            dscOptions.UseHangfireJobServer = true;
            dscOptions.UseSlackLogging = false;
            using (var app = WebApp.Start<OwinStartup>(options))
            {
                Console.WriteLine("Launching website...");
                Console.ReadLine();
                host.Dispose();
            }
        }
    }
}
