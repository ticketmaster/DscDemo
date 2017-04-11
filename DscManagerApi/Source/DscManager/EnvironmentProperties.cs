namespace Ticketmaster.Dsc.DscManager
{
    using System.Diagnostics;

    using Ticketmaster.Dsc.Interfaces;

    public class EnvironmentProperties : IEnvironmentProperties
    {
        public EnvironmentProperties()
        {
            this.SetDebugBuildFlag();
        }

        public bool DebugBuild { get; private set; }

        public bool DebuggerAttached => Debugger.IsAttached;

        [Conditional("DEBUG")]
        private void SetDebugBuildFlag()
        {
            this.DebugBuild = true;
        }
    }
}