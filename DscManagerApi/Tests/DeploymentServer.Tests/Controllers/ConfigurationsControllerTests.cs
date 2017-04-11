// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationsControllerTests.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// <summary>
//   Defines the ConfigurationsControllerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Microsoft.Data.Edm.Library;

namespace DeploymentServer.Tests.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Routing;

    using Hangfire;
    using Hangfire.Storage;
    using Hangfire.Storage.Monitoring;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.DeploymentServer.Controllers;
    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.DeploymentServer.RequestModels;
    using Ticketmaster.Dsc.DeploymentServer.Services;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    using HttpMethod = System.Net.Http.HttpMethod;

    /// <summary>
    /// The configurations controller tests.
    /// </summary>   
    [TestClass]
    public class ConfigurationsControllerTests
    {
        /// <summary>
        /// The builds.
        /// </summary>
        private List<Configuration> configs;

        /// <summary>
        /// The build views.
        /// </summary>
        private List<ConfigurationView> configViews;

        /// <summary>
        /// The config documents.
        /// </summary>
        private List<ConfigurationDocument> configDocuments;

        /// <summary>
        /// The controller.
        /// </summary>
        private ConfigurationsController controller;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        /// Gets or sets the configuration service.
        /// </summary>
        protected IConfigurationService ConfigurationService { get; set; }

        /// <summary>
        /// Gets or sets the deployment server logging.
        /// </summary>
        protected IDeploymentServerLogging DeploymentServerLogging { get; set; }

        /// <summary>
        /// Gets or sets the view model factory object.
        /// </summary>
        protected IViewModelFactory ViewModelFactoryObject { get; set; }

        /// <summary>
        /// Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        private Configuration TestConfigT1C1;

        private Configuration TestConfigT1C2;

        private Configuration TestConfigT2C1;

        private Configuration TestConfigT2C2;

        /// <summary>
        /// The delete succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task DeleteSucceeds()
        {
            var results = await this.controller.Delete(1);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
        }

        /// <summary>
        /// The delete entity not found.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task DeleteEntityNotFound()
        {
            var results = await this.controller.Delete(5);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }

        /// <summary>
        ///     The get all test.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task GetAllTest()
        {
            var results = this.controller.GetAll(new ODataQueryOptions<IGrouping<string, Configuration>>(new ODataQueryContext(new EdmModel(), typeof(Configuration)), new HttpRequestMessage()));
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            
            var resultContent = JsonConvert.SerializeObject(this.configViews);
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The get succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task GetSucceeds()
        {
            var results = this.controller.Get("target 1");
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.SerializeObject(this.configViews[0]);
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The get fails.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task GetEntityNotFound()
        {
            var results = this.controller.Get("target 3");
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The get document succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task GetDocumentSucceeds()
        {
            var results = await this.controller.GetDocument(1);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.SerializeObject(this.TestConfigT1C1.Map<ConfigurationDocumentView>());
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The get document not found.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task GetDocumentNotFound()
        {
            var results = await this.controller.GetDocument(5);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        /// The post succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task PostSucceeds()
        {
            var request = new ConfigurationRequest
                              {
                                  Document = this.TestConfigT1C1.ConfigurationDocument.Document,
                                  Target = this.TestConfigT1C1.Target
                              };
            var results = await this.controller.Post(new List<ConfigurationRequest> { request });
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.SerializeObject(request);
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The post enumerable succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task PostEnumerableSucceeds()
        {
            var requests = new List<ConfigurationRequest>();
            var request1 = new ConfigurationRequest
            {
                Document = this.TestConfigT1C1.ConfigurationDocument.Document,
                Target = this.TestConfigT1C1.Target
            };
            var request2 = new ConfigurationRequest
            {
                Document = this.TestConfigT2C1.ConfigurationDocument.Document,
                Target = this.TestConfigT2C1.Target
            };
            requests.Add(request1);
            requests.Add(request2);

            var results = await this.controller.Post(requests);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.SerializeObject(requests);
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The restore succeeds.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task RestoreSucceeds()
        {
            var results = await this.controller.Restore(2);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            this.configViews = new List<ConfigurationView>();

            var configView = new ConfigurationView(this.UrlHelper)
            {
                ConfigurationDocumentId = 3,
                Checksum = this.TestConfigT1C1.Checksum,
                Id = this.TestConfigT1C1.Id,
                PublishedTimestamp = this.TestConfigT1C1.PublishedTimestamp,
                Target = this.TestConfigT1C1.Target
            };

            var archiveViews = new List<ArchiveConfigurationView>();
            var archiveView = this.TestConfigT1C1.Map<ArchiveConfigurationView>();
            archiveViews.Add(archiveView);

            configView.ArchiveConfigurations = archiveViews;
            configView.PopulateLinks();

            var resultContent = JsonConvert.SerializeObject(configView);

            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The restore bad request.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task RestoreBadRequest()
        {
            var results = await this.controller.Restore(1);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.SerializeObject(new Hashtable
                              {
                                  { "Message", "The specified configuration is not an archived configuration." }
                              });
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        /// The restore entity not found.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        public async Task RestoreEntityNotFound()
        {
            var results = await this.controller.Restore(5);
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {

            this.MockUrlHelper();

            this.MockViewModelFactory();

            this.InitializeConfiguration();

            this.InitializeConfigurationViewAll();

            this.MockDbSet();

            this.MockConfigurationService();

            this.MockDeploymentServerLogging();

            this.controller = new ConfigurationsController(this.Context, this.ConfigurationService, this.DeploymentServerLogging)
            {
                Request
                                          =
                                          new HttpRequestMessage
                                          (
                                          HttpMethod
                                          .Get,
                                          "http://localhost/api/v2/configurations"),
                Configuration
                                          =
                                          new HttpConfiguration(),
                RequestContext =
                                          {
                                              RouteData
                                                  = new HttpRouteData(
                                                  new HttpRoute(),
                                                  new HttpRouteValueDictionary
                                                      {
                                                          {
                                                              "controller",
                                                              "configurations"
                                                          }
                                                      })
                                          }
            };

        }

        /// <summary>
        /// The generate checksum.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GenerateChecksum(string document)
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(document ?? string.Empty)))
                {
                    var sha = new SHA256Managed();
                    var checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", string.Empty);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The mock view model factory.
        /// </summary>
        private void MockViewModelFactory()
        {
            var factory = new Mock<IViewModelFactory>();
            factory.Setup(m => m.ConstructViewModel(It.IsAny<Type>()))
                .Returns((Type t) => Activator.CreateInstance(t, this.UrlHelper) as IViewModel);
            this.ViewModelFactoryObject = factory.Object;
            ViewModelFactory.Instance = this.ViewModelFactoryObject;
        }

        /// <summary>
        /// The initialize.
        /// </summary>       
        private void MockDbSet()
        {
            var mockSet = new Mock<DbSet<Configuration>>().SetupData(this.configs);
            mockSet.Setup(m => m.FindAsync(It.Is<int>(i => i == 1))).ReturnsAsync(this.TestConfigT1C1);
            mockSet.Setup(m => m.FindAsync(It.Is<int>(i => i == 2))).ReturnsAsync(this.TestConfigT1C2);
            mockSet.Setup(m => m.FindAsync(It.Is<int>(i => i == 3))).ReturnsAsync(this.TestConfigT2C1);
            mockSet.Setup(m => m.FindAsync(It.Is<int>(i => i == 4))).ReturnsAsync(this.TestConfigT2C2);
            var mockDocumentSet = new Mock<DbSet<ConfigurationDocument>>().SetupData(this.configDocuments);

            var context = new Mock<DeploymentServerContext>("nothing").UseDbSet(mockSet).UseDbSet(mockDocumentSet);

            this.Context = context.Object;
        }

        /// <summary>
        /// The initialize configuration.
        /// </summary>
        private void InitializeConfiguration()
        {
            this.configs = new List<Configuration>();
            this.configDocuments = new List<ConfigurationDocument>();
            var publishTime = DateTime.UtcNow.AddHours(-1);
            var archiveTime = DateTime.UtcNow;

            var target1 = "target 1";
            var mof1 = "Mof Document 1";
            var configDocument1 = new ConfigurationDocument { Document = mof1, Id = 1 };
            this.configDocuments.Add(configDocument1);
            this.TestConfigT1C1 = new Configuration
            {
                ConfigurationDocument = configDocument1,
                ConfigurationDocumentId = configDocument1.Id,
                Id = 1,
                PublishedTimestamp = publishTime,
                Target = target1,
                Checksum = GenerateChecksum(mof1)
            };

            var target2 = "target 2";
            var mof2 = "Mof Document 2";
            var configDocument2 = new ConfigurationDocument { Document = mof2, Id = 2 };
            this.configDocuments.Add(configDocument2);
            this.TestConfigT2C1 = new Configuration
            {
                ConfigurationDocument = configDocument2,
                ConfigurationDocumentId = configDocument2.Id,
                Id = 2,
                PublishedTimestamp = publishTime,
                Target = target2,
                Checksum = GenerateChecksum(mof2)
            };

            var mof3 = "Mof Document 3";
            var configDocument3 = new ConfigurationDocument { Document = mof3, Id = 3 };
            this.configDocuments.Add(configDocument3);
            this.TestConfigT1C2 = new Configuration
            {
                ArchiveTimestamp = archiveTime,
                ConfigurationDocument = configDocument3,
                ConfigurationDocumentId = configDocument3.Id,
                Id = 1,
                PublishedTimestamp = publishTime,
                Target = target1,
                Checksum = GenerateChecksum(mof1)
            };

            var mof4 = "Mof Document 4";
            var configDocument4 = new ConfigurationDocument { Document = mof4, Id = 4 };
            this.configDocuments.Add(configDocument4);
            this.TestConfigT2C2 = new Configuration
            {
                ArchiveTimestamp = archiveTime,
                ConfigurationDocument = configDocument4,
                ConfigurationDocumentId = configDocument4.Id,
                Id = 2,
                PublishedTimestamp = publishTime,
                Target = target2,
                Checksum = GenerateChecksum(mof2)
            };

            this.configs.Add(this.TestConfigT1C1);
            this.configs.Add(this.TestConfigT1C2);
            this.configs.Add(this.TestConfigT2C1);
            this.configs.Add(this.TestConfigT2C2);
        }

        /// <summary>
        /// The initialize configuration view all.
        /// </summary>
        private void InitializeConfigurationViewAll()
        {
            this.configViews = new List<ConfigurationView>();

            var configView1 = new ConfigurationView(this.UrlHelper)
                                  {
                                      ConfigurationDocumentId = this.TestConfigT1C1.ConfigurationDocumentId,
                                      Checksum = this.TestConfigT1C1.Checksum,
                                      Id = this.TestConfigT1C1.Id,
                                      PublishedTimestamp = this.TestConfigT1C1.PublishedTimestamp,
                                      Target = this.TestConfigT1C1.Target
                                  };

            var archiveViews1 = new List<ArchiveConfigurationView>();
            var archiveView1 = this.TestConfigT1C2.Map<ArchiveConfigurationView>();
            archiveViews1.Add(archiveView1);

            configView1.ArchiveConfigurations = archiveViews1;
            configView1.PopulateLinks();

            this.configViews.Add(configView1);

            var configView2 = new ConfigurationView(this.UrlHelper)
            {
                ConfigurationDocumentId = this.TestConfigT2C1.ConfigurationDocumentId,
                Checksum = this.TestConfigT2C1.Checksum,
                Id = this.TestConfigT2C1.Id,
                PublishedTimestamp = this.TestConfigT2C1.PublishedTimestamp,
                Target = this.TestConfigT2C1.Target
            };

            var archiveViews2 = new List<ArchiveConfigurationView>();
            var archiveView2 = this.TestConfigT2C2.Map<ArchiveConfigurationView>();
            archiveViews2.Add(archiveView2);

            configView2.ArchiveConfigurations = archiveViews2;
            configView2.PopulateLinks();
            this.configViews.Add(configView2);
        }

        /// <summary>
        /// The initialize configuration view target 1.
        /// </summary>
        private void InitializeConfigurationViewTarget1()
        {
            this.configViews = new List<ConfigurationView>();

            var configView1 = new ConfigurationView(this.UrlHelper)
            {
                ConfigurationDocumentId = this.TestConfigT1C1.ConfigurationDocumentId,
                Checksum = this.TestConfigT1C1.Checksum,
                Id = this.TestConfigT1C1.Id,
                PublishedTimestamp = this.TestConfigT1C1.PublishedTimestamp,
                Target = this.TestConfigT1C1.Target
            };

            var archiveViews1 = new List<ArchiveConfigurationView>();
            var archiveView1 = this.TestConfigT1C2.Map<ArchiveConfigurationView>();
            archiveViews1.Add(archiveView1);

            configView1.ArchiveConfigurations = archiveViews1;
            configView1.PopulateLinks();

            this.configViews.Add(configView1); 
        }

        /// <summary>
        /// The initialize configuration view.
        /// </summary>
        private void InitializeConfigurationView()
        {                   
            this.configViews = new List<ConfigurationView>();
            var archiveViews = new List<ArchiveConfigurationView>();
            var configView = new ConfigurationView(this.UrlHelper);




            foreach (var config in this.configs)
            {
                if (config.ArchiveTimestamp == null)
                {

                    configView.Checksum = config.Checksum;
                    configView.Id = config.Id;
                    configView.Target = config.Target;
                    configView.ConfigurationDocumentId = config.ConfigurationDocumentId;
                    configView.PublishedTimestamp = config.PublishedTimestamp;
                                                                                      
                    configView.PopulateLinks();
                    
                }
                else
                {
                    var archiveView = new ArchiveConfigurationView(this.UrlHelper)
                                          {
                                              ArchiveTimestamp = config.ArchiveTimestamp ?? DateTime.UtcNow,
                                              Checksum = config.Checksum,
                                              Id = config.Id,
                                              Target = config.Target,
                                              ConfigurationDocumentId =
                                                  config.ConfigurationDocumentId,
                                              PublishedTimestamp =
                                                  config.PublishedTimestamp
                                          };
                    archiveView.PopulateLinks();
                    archiveViews.Add(archiveView);
                }

                configView.ArchiveConfigurations = archiveViews;
                this.configViews.Add(configView);
            }
        }

        /// <summary>
        /// The mock configuration service.
        /// </summary>
        private void MockConfigurationService()
        {
            var configService = new Mock<IConfigurationService>();

            this.ConfigurationService = configService.Object;
        }

        /// <summary>
        /// The mock deployment server logging.
        /// </summary>
        private void MockDeploymentServerLogging()
        {
            var logging = new Mock<IDeploymentServerLogging>();

            this.DeploymentServerLogging = logging.Object;
        }

        /// <summary>
        /// The mock url helper.
        /// </summary>
        private void MockUrlHelper()
        {
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns(string.Empty);
            urlHelper.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);
            this.UrlHelper = urlHelper.Object;
        }

    }
}
