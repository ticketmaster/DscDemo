using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DebugLauncher;
using Microsoft.Owin;

//[assembly: OwinStartup(typeof(OwinStartup))]
namespace DebugLauncher
{
    using System.Net;

    using Owin;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Extensions;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Extensions;

    /*public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpListener = (HttpListener)app.Properties["System.Net.HttpListener"];
            httpListener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            var options = new DeploymentServerOptions
                              {
                                  ConfigurationWorkingPath =
                                      @"C:\users\mike.walker\desktop\dscm\work",
                                  RootConfigurationPath = @"C:\users\mike.walker\desktop\dscm\RootConfiguration.ps1",
                                  UseJobDashbaord = true
                              };

            //app.UseDeploymentServer(options);
            app.UseNodeRepository(new NodeRepositoryOptions());
        }
    }*/
}
