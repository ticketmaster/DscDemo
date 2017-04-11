namespace Ticketmaster.Dsc.DeploymentServer.Services
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The build grouping.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class BuildGrouping<T> : List<T>, IGrouping<BuildGroupKey, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGrouping{T}"/> class.
        /// </summary>
        public BuildGrouping()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGrouping{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        public BuildGrouping(string name, string version)
        {
            this.Key.Name = name;
            this.Key.Version = version;
        }

        /// <summary>Gets the key of the <see cref="T:System.Linq.IGrouping`2" />.</summary>
        /// <returns>The key of the <see cref="T:System.Linq.IGrouping`2" />.</returns>
        public BuildGroupKey Key { get; } = new BuildGroupKey();
    }
}