// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSetExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;

    /// <summary>
    ///     The db set extensions.
    /// </summary>
    public static class DbSetExtensions
    {
        /*public static IEnumerable<T> ToViewModels<T>(this IEnumerable<IModel<IViewModel>> models) where T : IViewModel
        {
            return models.Select(model => model.ToViewModel<T>()).ToList();
        }*/

        /// <summary>
        /// The get properties for node.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ConfigurationProperty> GetPropertiesForNode(
            this DbSet<ConfigurationProperty> repository, 
            PropertyType propertyType, 
            Node node)
        {
            var roles = node.Roles.GetRoleNames();
            return
                repository.Where(
                    p =>
                    p.Type == propertyType
                    && (p.Scope == PropertyScope.Global || (p.Scope == PropertyScope.Site && p.Target == node.Site)
                        || (p.Scope == PropertyScope.ConfigurationEnvironment
                            && p.Target == node.ConfigurationEnvironment)
                        || (p.Scope == PropertyScope.Role && roles.Contains(p.Target))
                        || (p.Scope == PropertyScope.Node && p.Target == node.Name))).ResolveScope();
        }

        /// <summary>
        /// The get properties for node.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ConfigurationProperty> GetPropertiesForNode(
            this DbSet<ConfigurationProperty> repository, 
            Node node)
        {
            var roles = node.Roles.GetRoleNames();
            return
                repository.Where(
                    p =>
                    p.Scope == PropertyScope.Global || (p.Scope == PropertyScope.Site && p.Target == node.Site)
                    || (p.Scope == PropertyScope.ConfigurationEnvironment && p.Target == node.ConfigurationEnvironment)
                    || (p.Scope == PropertyScope.Role && roles.Contains(p.Target))
                    || (p.Scope == PropertyScope.Node && p.Target == node.Name)).ResolveScope();
        }

        public static IEnumerable<ConfigurationProperty> GetPropertiesForNode(
            this IEnumerable<ConfigurationProperty> repository,
            Node node)
        {
            var roles = node.Roles.GetRoleNames();
            return
                repository.Where(
                    p =>
                    p.Scope == PropertyScope.Global || (p.Scope == PropertyScope.Site && p.Target == node.Site)
                    || (p.Scope == PropertyScope.ConfigurationEnvironment && p.Target == node.ConfigurationEnvironment)
                    || (p.Scope == PropertyScope.Role && roles.Contains(p.Target))
                    || (p.Scope == PropertyScope.Node && p.Target == node.Name)).ResolveScope();
        }

        /*public static T ToViewModel<T>(this IEnumerable<ConfigurationProperty> properties) where T : IViewModel
        {
            var viewModel = Activator.CreateInstance<T>();
            return ToViewModel(properties, viewModel);
        }

        public static T ToViewModel<T>(this IEnumerable<ConfigurationProperty> properties, T viewModel) where T : IViewModel
        {
            var modelProperties = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var modelProp = modelProperties.FirstOrDefault(p => p.Name == property.Name);
                if (modelProp == null)
                {
                    continue;
                }

                try
                {
                    modelProp.SetValue(viewModel, property.Value);
                }
                catch
                {
                    continue;
                }
            }

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(viewModel, new ValidationContext(viewModel), validationResults))
            {
                return default(T);
            }

            return viewModel;
        }*/

        /// <summary>
        /// The get role names.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<string> GetRoleNames(this IEnumerable<Role> roles)
        {
            return roles.Select(r => r.Name);
        }

        public static void IncludeConfigurationProperties(
            this IEnumerable<Node> nodes,
            NodeRepositoryContext context,
            bool force = false)
        {
            var allProperties = context.Set<ConfigurationProperty>();
            foreach (var node in nodes)
            {
                if (node.ConfigurationPropertiesPopulated && !force)
                {
                    continue;
                }

                var properties = allProperties.GetPropertiesForNode(node);
                foreach (var property in properties)
                {
                    switch (property.Type)
                    {
                        case PropertyType.Node:
                            if (node.NodeProperties.All(p => p.Id != property.Id))
                            {
                                node.NodeProperties.Add(property);
                            }

                            break;
                        case PropertyType.LocalAgent:
                            if (node.LocalAgentProperties.All(p => p.Id != property.Id))
                            {
                                node.LocalAgentProperties.Add(property);
                            }

                            break;
                        case PropertyType.Bootstrap:
                            if (node.BootstrapProperties.All(p => p.Id != property.Id))
                            {
                                node.BootstrapProperties.Add(property);
                            }

                            break;
                        case PropertyType.ResourceVersion:
                            if (node.ResourceVersionProperties.All(r => r.Id != property.Id))
                            {
                                node.ResourceVersionProperties.Add(property);
                            }

                            break;
                    }
                }
            }
        }
        /// <summary>
        /// The populate configuration properties.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="force">
        /// Whether to force reloading of properties
        /// </param>
        public static void IncludeConfigurationProperties(this Node node, NodeRepositoryContext context, bool force = false)
        {
            if (node.ConfigurationPropertiesPopulated && !force)
            {
                return;
            }

            var properties = context.Set<ConfigurationProperty>().GetPropertiesForNode(node);
            foreach (var property in properties)
            {
                switch (property.Type)
                {
                    case PropertyType.Node:
                        if (node.NodeProperties.All(p => p.Id != property.Id))
                        {
                            node.NodeProperties.Add(property);
                        }

                        break;
                    case PropertyType.LocalAgent:
                        if (node.LocalAgentProperties.All(p => p.Id != property.Id))
                        {
                            node.LocalAgentProperties.Add(property);
                        }

                        break;
                    case PropertyType.Bootstrap:
                        if (node.BootstrapProperties.All(p => p.Id != property.Id))
                        {
                            node.BootstrapProperties.Add(property);
                        }

                        break;
                    case PropertyType.ResourceVersion:
                        if (node.ResourceVersionProperties.All(r => r.Id != property.Id))
                        {
                            node.ResourceVersionProperties.Add(property);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// The resolve scope.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ConfigurationProperty> ResolveScope(
            this IEnumerable<ConfigurationProperty> properties)
        {
            var resolvedProperties = new List<ConfigurationProperty>();
            var typeGroups = properties.GroupBy(p => p.Type);
            foreach (var groups in typeGroups.Select(tGroup => tGroup.GroupBy(p => p.Name)))
            {
                resolvedProperties.AddRange(
                    groups.Select(group => group.OrderByDescending(g => g.Scope).FirstOrDefault()));
            }

            return resolvedProperties;
        }
    }
}