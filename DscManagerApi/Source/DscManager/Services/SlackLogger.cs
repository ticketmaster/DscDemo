using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System.Drawing;

    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Integrations.Slack.Models;
    using Ticketmaster.Integrations.Slack.Services;

    public class SlackLogger : DscEventHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DscEventHandler"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        public SlackLogger(IDscEventManager eventManager, ISlackIntegrationService slackService)
            : base(eventManager)
        {
            this.SlackService = slackService;
        }

        protected ISlackIntegrationService SlackService { get; set; }

        /// <summary>
        /// The handle event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task HandleEvent(DscEventArgs eventArgs)
        {
            var message = new SlackMessage();
            var attachment = new SlackAttachment();
            message.Attachments.Add(attachment);
            switch (eventArgs.Name)
            {
                case "MofBuildSucceeded":
                    attachment.Text = "<https://dsc.winsys.tmcs/api/v2/builds/" + eventArgs.GetMember("BuildId") + "|Mof build #" + eventArgs.GetMember("BuildId") + "> has completed successfully.";
                    attachment.Color = Color.Green;
                    break;
                case "MofBuildFailed":
                    attachment.Text = "<https://dsc.winsys.tmcs/api/v2/builds/" + eventArgs.GetMember("BuildId") + "|Mof build #" + eventArgs.GetMember("BuildId") + "> has failed.";
                    attachment.Color = Color.Red;
                    break;
                case "NodeChangedInitialDeployment":
                    var target = eventArgs.GetMember<string>("name");
                    var newValue = eventArgs.GetMember<bool>("status");
                    if (!newValue)
                    {
                        attachment.Text = "Node *" + target + "* has successfully completed initial deployment.";
                        attachment.Color = Color.LightGray;
                    }
                    break;
                case "NodeAgentError":
                    var name = eventArgs.GetMember<string>("name");
                    attachment.Text = "Node *" + name + "* has reported the following error: " + eventArgs.GetMember<string>("errorMessage") + ".";
                    attachment.Color = Color.Red;
                    break;
                case "SubmittedBuildRequest":
                    attachment.Text = "<https://dsc.winsys.tmcs/api/v2/builds/" + eventArgs.GetMember("BuildId") + "|Mof build #" + eventArgs.GetMember("BuildId") + "> has been submitted. " + eventArgs.GetMember("TargetCount") + " node(s) will be built.";
                    attachment.Color = Color.LightGray;
                    break;
                default:
                    return Task.FromResult<object>(null);
            }

            this.SlackService.SendMessage(message);
            return Task.FromResult<object>(null);
        }
    }
}
