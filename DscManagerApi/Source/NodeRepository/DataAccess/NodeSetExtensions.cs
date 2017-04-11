// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeSetExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataAccess
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.NodeRepository.DataModels;

    /// <summary>
    ///     The node set extensions.
    /// </summary>
    public static class NodeSetExtensions
    {
        /// <summary>
        /// The find node by name.
        /// </summary>
        /// <param name="dbSet">
        /// The db set.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<Node> FindNodeByName(this IQueryable<Node> dbSet, string name)
        {
            return await dbSet.FirstOrDefaultAsync(n => n.Name == name || n.NodeName == name || n.QualifiedName == name);
        }
    }
}