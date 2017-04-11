using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DscManager.Service
{
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(
                c =>
                    {
                        c.Service(hostSettings => new DscManagerService(hostSettings));
                        c.SetDisplayName("DscManager API");
                        c.SetServiceName("DscManager");
                        c.SetDescription("The DscManager API provides an API to manage DSC nodes.");
                        c.StartAutomatically();
                        c.EnableServiceRecovery(
                            rc =>
                                {
                                    rc.RestartService(1);
                                    rc.RestartService(1);
                                    rc.SetResetPeriod(1);
                                });
                        c.RunAsPrompt();
                        c.BeforeInstall(DscManagerService.Install);
                        c.AfterUninstall(DscManagerService.Uninstall);
                    });
        }
    }
}
