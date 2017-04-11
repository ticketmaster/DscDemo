using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DscManager.Service
{
    using System.Diagnostics;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using Microsoft.Owin.Hosting;

    using Topshelf;
    using Topshelf.Runtime;

    public class DscManagerService : ServiceControl, IDisposable
    {
        public DscManagerService(HostSettings settings)
        {
            this.Settings = settings;
            var roles = settings.InstanceName.ToLower().Split(",".ToCharArray());
            foreach (var role in roles)
            {
                switch (role)
                {
                    case "dscmanager":
                        this.UseDscManagerApi = true;
                        Console.WriteLine("Role: DscManager detected. The DscManager API will be launched.");
                        break;
                    case "jobserver":
                        this.UseJobServer = true;
                        Console.WriteLine("Role: JobServer detected. The JobServer component of DscManager will be launched.");
                        break;
                    case "webrepository":
                        this.UseWebRepository = true;
                        Console.WriteLine("Role: WebRepository detected. The WebRepository API will be launched.");
                        break;
                    default:
                        Console.WriteLine("Role: " + role + " is not recognized.");
                        break;
                }
            }
        }

        protected HostSettings Settings { get; set; }

        protected bool UseJobServer { get; set; }

        protected bool UseWebRepository { get; set; }

        protected bool UseDscManagerApi { get; set; }

        protected IDisposable WebRepositoryApp { get; set; }
        protected IDisposable DscManagerApp { get; set; }

        public bool Start(HostControl hostControl)
        {
            hostControl.RequestAdditionalTime(TimeSpan.FromSeconds(60));

            if (this.UseWebRepository)
            {
                Console.WriteLine("Launching WebRepository...");
                var options = new StartOptions();
                options.Urls.Add("http://+:80/");
                this.WebRepositoryApp = WebApp.Start<Ticketmaster.Dsc.WebRepository.OwinStartup>(options);
            }

            if (this.UseDscManagerApi || this.UseJobServer)
            {
                var dscOptions = Ticketmaster.Dsc.DscManager.OwinStartup.GetDscManagerOptions();
                dscOptions.UseApi = this.UseDscManagerApi;
                dscOptions.UseHangfireJobServer = this.UseJobServer;
                var options = new StartOptions();
                options.Urls.Add("https://+:443");
                this.DscManagerApp = WebApp.Start<Ticketmaster.Dsc.DscManager.OwinStartup>(options);
            }

            Console.WriteLine("Finished loading application. Press any key to exit.");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (this.DscManagerApp != null)
            {
                this.DscManagerApp.Dispose();
                this.DscManagerApp = null;
            }

            if (this.WebRepositoryApp != null)
            {
                this.WebRepositoryApp.Dispose();
                this.WebRepositoryApp = null;
            }

            return true;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Stop(null);
        }

        public static void Install()
        {
            /*var info = new List<Process>
                           {
                               Process.Start(
                                   "cmd.exe",
                                   "/c \"netsh http add urlacl url=https://+:443/ user=Everyone\""),
                               Process.Start(
                                   "cmd.exe",
                                   "/c \"netsh http add sslcert ipport=0.0.0.0:443 certhash=378F49E405A3BC281FE20AA712621B3318A34110 appid={f2bd7456-bd83-4043-bf0a-01defba3e48e}\"")
                           };
            do
            {
                Thread.Sleep(200);
            }
            while (info.Any(i => i.HasExited == false));*/
        }

        public static void Uninstall()
        {
            /*var info = new List<Process>
                           {
                               Process.Start(
                                   "cmd.exe",
                                   "/c \"netsh http delete urlacl url=https://+:443/\""),
                               Process.Start(
                                   "cmd.exe",
                                   "/c \"netsh http delete sslcert ipport=0.0.0.0:443\"")
                           };
            do
            {
                Thread.Sleep(200);
            }
            while (info.Any(i => i.HasExited == false));*/
        }
    }
}
