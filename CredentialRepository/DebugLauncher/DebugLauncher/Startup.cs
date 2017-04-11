using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugLauncher
{
    using Microsoft.Owin.Hosting;

    public class Startup
    {
        static void Main(string[] args)
        {
            var options = new StartOptions();
            options.Urls.Add("http://+:80/");
            using (var app = WebApp.Start<OwinStartup>(options))
            {
                Console.WriteLine("Launching website...");
                Console.ReadLine();
            }
        }
    }
}
