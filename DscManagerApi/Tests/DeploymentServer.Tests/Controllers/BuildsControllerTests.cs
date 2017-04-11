using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Web.Http.Routing;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Data.Edm.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Ticketmaster.Dsc.DeploymentServer.Controllers;
using Ticketmaster.Dsc.DeploymentServer.DataAccess;
using Ticketmaster.Dsc.DeploymentServer.DataModels;
using Ticketmaster.Dsc.DeploymentServer.RequestModels;
using Ticketmaster.Dsc.Interfaces.DeploymentServer;
using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
using Ticketmaster.Dsc.Interfaces.Mapping;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DeploymentServer.Tests.Controllers
{
    /// <summary>
    ///     The builds controller tests.
    /// </summary>
    [TestClass]
    public class BuildsControllerTests
    {
        /// <summary>
        ///     The build detail views.
        /// </summary>
        private List<BuildDetailView> buildDetailViews;

        /// <summary>
        ///     The builds.
        /// </summary>
        private List<Build> builds;

        /// <summary>
        ///     The build views.
        /// </summary>
        private List<BuildView> buildViews;

        /// <summary>
        ///     The controller.
        /// </summary>
        private BuildsController controller;

        /// <summary>
        ///     The targets.
        /// </summary>
        private List<BuildTarget> targets;

        /// <summary>
        ///     Gets or sets the background job server.
        /// </summary>
        protected BackgroundJobServer BackgroundJobServer { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the mof builder service.
        /// </summary>
        protected IMofBuilderService MofBuilderService { get; set; }

        /// <summary>
        ///     Gets or sets the monitor api mock.
        /// </summary>
        protected Mock<IMonitoringApi> MonitorApiMock { get; set; }

        /// <summary>
        ///     Gets or sets the monitoring Api.
        /// </summary>
        protected IMonitoringApi MonitoringApi { get; set; }

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     Gets or sets the test ids.
        /// </summary>
        private int BuildCount { get; set; }

        /// <summary>
        ///     Gets or sets the target count.
        /// </summary>
        private int TargetCount { get; set; }

        /// <summary>
        ///     Gets or sets the view.
        /// </summary>
        private BuildDetailView View { get; set; }

        protected IViewModelFactory ViewModelFactoryObject { get; set; }

        protected JobDetailsDto StateHistoryWithException { get; set; }

        protected JobDetailsDto StateHistoryNotFound { get; set; }

        /// <summary>
        ///     The cancel succeeds.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task CancelSucceeds()
        {
            var id = 1;
            var results = this.controller.Cancel(id).Result;
            var result = await results.ExecuteAsync(CancellationToken.None);

            var resultContent = JsonConvert.SerializeObject(this.View);

            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNull(result.Content);
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
            var results = this.controller.GetAll(new ODataQueryOptions<Build>(new ODataQueryContext(new EdmModel(), typeof(Build)), new HttpRequestMessage()));
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.SerializeObject(this.buildViews);
            Assert.AreEqual(content, resultContent);
        }

        /// <summary>
        ///     The get entity not found.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task GetEntityNotFound()
        {
            var results = this.controller.Get(12).Result;
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }

        /// <summary>
        ///     The get succeeds.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task GetSucceeds()
        {
            for (var i = 1; i <= this.BuildCount; i++)
            {
                var results = this.controller.Get(i).Result;
                var result = await results.ExecuteAsync(CancellationToken.None);
                Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
                var content = await result.Content.ReadAsStringAsync();
                var resultContent = JsonConvert.SerializeObject(this.buildDetailViews.FirstOrDefault(d => d.Id == i));
                Assert.AreEqual(resultContent, content);
            }
        }

        /// <summary>
        ///     The get without task history.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task GetWithoutTaskHistory()
        {
            var id = 1;
            this.MockMonitorApiWithoutHistory();
            var results = this.controller.Get(id).Result;
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            var expectedView = this.buildDetailViews.FirstOrDefault(d => d.Id == id);
            var jobView = new JobView {Id = 10};
            var job = this.StateHistoryNotFound;
            if (job.History.Any())
            {
                if (job.History[0].Data.ContainsKey("ExceptionMessage"))
                {
                    jobView.Message = job.History[0].Data["ExceptionMessage"];
                }

                if (job.History[0].Data.ContainsKey("ExceptionDetails"))
                {
                    jobView.MessageDetailed = job.History[0].Data["ExceptionDetails"];
                }

                jobView.Status = job.History[0].StateName;
            }

            Assert.IsNotNull(expectedView);
            expectedView.Jobs = new List<JobView> {jobView};
            var resultContent = JsonConvert.SerializeObject(expectedView);
            Assert.AreEqual(resultContent, content);

            // Restore Mock
            this.MockMonitorApi();
        }

        /// <summary>
        ///     The get with task exception.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task GetWithTaskException()
        {
            var id = 1;
            this.MockMonitorApiWithException();
            var results = this.controller.Get(id).Result;
            var result = await results.ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            var expectedView = this.buildDetailViews.FirstOrDefault(d => d.Id == id);
            var jobView = new JobView {Id = 10};
            var job = this.StateHistoryWithException;
            if (job.History.Any())
            {
                if (job.History[0].Data.ContainsKey("ExceptionMessage"))
                {
                    jobView.Message = job.History[0].Data["ExceptionMessage"];
                }

                if (job.History[0].Data.ContainsKey("ExceptionDetails"))
                {
                    jobView.MessageDetailed = job.History[0].Data["ExceptionDetails"];
                }

                jobView.Status = job.History[0].StateName;
            }

            Assert.IsNotNull(expectedView);
            expectedView.Jobs = new List<JobView> {jobView};
            var resultContent = JsonConvert.SerializeObject(expectedView);
            Assert.AreEqual(resultContent, content);

            // Restore Mock
            this.MockMonitorApi();
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.MockUrlHelper();

            this.MockViewModelFactory();

            this.InitializeBuildDetailView(10, 1);

            this.MockDbSet();

            this.MockBackgroundJobServer();

            this.MockMonitorApi();

            this.MockMofBuilderService();

            this.controller = new BuildsController(this.Context, this.MonitoringApi, this.MofBuilderService)
            {
                Request
                    =
                    new HttpRequestMessage
                        (
                        HttpMethod
                            .Get,
                        "http://localhost/api/v2/builds"),
                Configuration
                    =
                    new HttpConfiguration
                        (),
                RequestContext
                    =
                {
                    RouteData
                        =
                        new HttpRouteData
                            (
                            new HttpRoute
                                (
                                ),
                            new HttpRouteValueDictionary
                            {
                                {
                                    "controller",
                                    "builds"
                                }
                            })
                }
            };
        }

        private void MockViewModelFactory()
        {
            var factory = new Mock<IViewModelFactory>();
            factory.Setup(m => m.ConstructViewModel(It.IsAny<Type>()))
                .Returns((Type t) => Activator.CreateInstance(t, this.UrlHelper) as IViewModel);
            this.ViewModelFactoryObject = factory.Object;
            ViewModelFactory.Instance = this.ViewModelFactoryObject;
        }

        /// <summary>
        ///     The post test.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [TestMethod]
        public async Task PostSucceeds()
        {
            var results = this.controller.Post(new BuildRequest()).Result;
            var result = await results.ExecuteAsync(CancellationToken.None);

            var content = await result.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.SerializeObject(this.View);

            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(resultContent, content);
        }

        /// <summary>
        ///     The initialize build controller.
        /// </summary>
        /// <param name="buildCount">
        ///     The buildCount.
        /// </param>
        /// <param name="targetCount">
        ///     The target Count.
        /// </param>
        private void InitializeBuildDetailView(int buildCount = 1, int targetCount = 1)
        {
            this.builds = new List<Build>();
            this.targets = new List<BuildTarget>();
            this.buildDetailViews = new List<BuildDetailView>();
            this.buildViews = new List<BuildView>();
            this.BuildCount = buildCount;
            this.TargetCount = targetCount;
            var rng = new Random();

            for (var i = 1; i <= buildCount; i++)
            {
                var build = new Build
                {
                    Id = i,
                    RequestTimestamp = DateTime.UtcNow.AddHours(rng.Next(-72, 0)),
                    CompleteTimestamp = DateTime.UtcNow,
                    Status = (BuildStatus) rng.Next(0, 3)
                };
                var buildView = new BuildView(this.UrlHelper)
                {
                    Id = i,
                    RequestTimestamp = build.RequestTimestamp,
                    CompleteTimestamp = build.CompleteTimestamp,
                    Status = build.Status
                };
                buildView.PopulateLinks();
                this.buildViews.Add(buildView);
                var buildDetailView = new BuildDetailView(this.UrlHelper)
                {
                    Id = i,
                    RequestTimestamp = build.RequestTimestamp,
                    CompleteTimestamp = build.CompleteTimestamp,
                    Status = build.Status
                };
                buildDetailView.PopulateLinks();

                build.Targets = new List<BuildTarget>();
                var targetViews = new List<BuildTargetView>();

                for (var tc = 1; tc <= targetCount; tc++)
                {
                    var job = rng.Next(1, 100);
                    build.Targets.Add(new BuildTarget {Id = i, Build = build, JobId = 10, Target = "Target" + tc});
                    targetViews.Add(new BuildTargetView {Target = "Target" + tc});
                }

                this.targets.AddRange(build.Targets);
                this.builds.Add(build);
                buildDetailView.Targets = targetViews;
                this.buildDetailViews.Add(buildDetailView);
            }
        }

        /// <summary>
        ///     The mock mapper.
        /// </summary>
        /// <summary>
        ///     The mock background job server.
        /// </summary>
        private void MockBackgroundJobServer()
        {
            var storage = new MyJobStorage();
            var options = new BackgroundJobServerOptions();

            // var supervisor = new Mock<IServerSupervisor>();
            var server = new Mock<BackgroundJobServer>(options, storage) {CallBase = true};

            this.BackgroundJobServer = server.Object;
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        private void MockDbSet()
        {
            var mockSet = new Mock<DbSet<Build>>().SetupData(this.builds);
            mockSet.Setup(m => m.FindAsync(It.IsAny<object>()))
                .Returns((object p) => Task.FromResult(this.builds.FirstOrDefault(t => t.Id == (int)p)));
            var mockTargetSet = new Mock<DbSet<BuildTarget>>().SetupData(this.targets);

            var context = new Mock<DeploymentServerContext>("nothing").UseDbSet(mockSet).UseDbSet(mockTargetSet);

            this.Context = context.Object;
        }

        /// <summary>
        ///     The mock mof builder service.
        /// </summary>
        private void MockMofBuilderService()
        {
            var mofBuilder = new Mock<IMofBuilderService>();
            this.View = new BuildDetailView(this.UrlHelper)
            {
                CompleteTimestamp = DateTime.UtcNow,
                Id = 1,
                RequestTimestamp = DateTime.UtcNow,
                Status = BuildStatus.Enqueued,
                Jobs = new List<JobView>(),
                Links = new List<Link>(),
                Targets = new List<BuildTargetView>()
            };
            mofBuilder.Setup(m => m.Build(It.IsAny<Hashtable>()))
                .Returns(() => Task.FromResult(this.View));

            this.MofBuilderService = mofBuilder.Object;
        }

        /// <summary>
        ///     The mock monitor api.
        /// </summary>
        private void MockMonitorApi()
        {
            this.MonitorApiMock = new Mock<IMonitoringApi>();

            this.MonitoringApi = this.MonitorApiMock.Object;
        }

        /// <summary>
        ///     The mock monitor api with exception.
        /// </summary>
        private void MockMonitorApiWithException()
        {
            var sh = new StateHistoryDto
            {
                Data =
                    new Dictionary<string, string>
                    {
                        {
                            "ExceptionMessage",
                            "string message"
                        },
                        {
                            "ExceptionDetails",
                            "string details"
                        }
                    },
                CreatedAt = DateTime.Now,
                Reason = "A reason",
                StateName = "BadState"
            };

            var jd = new JobDetailsDto {CreatedAt = DateTime.Now, History = new List<StateHistoryDto> {sh}};
            this.StateHistoryWithException = jd;
            this.MonitorApiMock.Setup(m => m.JobDetails(It.IsAny<string>())).Returns(jd);
        }

        /// <summary>
        ///     The mock monitor api without history.
        /// </summary>
        private void MockMonitorApiWithoutHistory()
        {
            var jd = new JobDetailsDto {CreatedAt = DateTime.Now, History = new List<StateHistoryDto>()};
            this.StateHistoryNotFound = jd;
            this.MonitorApiMock.Setup(m => m.JobDetails(It.IsAny<string>())).Returns(jd);
        }

        /// <summary>
        ///     The mock url helper.
        /// </summary>
        private void MockUrlHelper()
        {
            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(string.Empty);
            urlHelper.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);
            this.UrlHelper = urlHelper.Object;
        }

        /// <summary>
        ///     The my job storage.
        /// </summary>
        private class MyJobStorage : JobStorage
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="MyJobStorage" /> class.
            /// </summary>
            public MyJobStorage()
            {
                Current = this;
            }

            /// <summary>
            ///     The get connection.
            /// </summary>
            /// <returns>
            ///     The <see cref="IStorageConnection" />.
            /// </returns>
            public override IStorageConnection GetConnection()
            {
                var mock = new Mock<IStorageConnection>();
                return mock.Object;
            }

            /// <summary>
            ///     The get monitoring api.
            /// </summary>
            /// <returns>
            ///     The <see cref="IMonitoringApi" />.
            /// </returns>
            public override IMonitoringApi GetMonitoringApi()
            {
                var mock = new Mock<IMonitoringApi>();
                return mock.Object;
            }
        }
    }
}