using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketmaster.Dsc.DeploymentServer.Services
{
    using System.Management.Automation;

    public class MofBuildException : Exception
    {
        public MofBuildException(IEnumerable<ErrorRecord> errorRecords)
        {
            var enumerable = errorRecords as ErrorRecord[] ?? errorRecords.ToArray();
            if (errorRecords != null)
            {
                this.Message = "The MOF build job has failed.\r\n"
                               + string.Join("\r\n\r\n", enumerable.Select(e => e.ToString()));
            }
            else
            {
                this.Message = "The MOF build job has failed.";
            }

            this.ErrorRecords = enumerable;
        }

        /// <summary>Gets a message that describes the current exception.</summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
        public override string Message { get; }

        public IEnumerable<ErrorRecord> ErrorRecords { get; set; }
    }
}
