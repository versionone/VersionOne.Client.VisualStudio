using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ninject;
using NUnit.Framework;
using VersionOne.SDK.APIClient;
using VersionOne.SDK.ObjectModel;
using VersionOne.SDK.ObjectModel.Filters;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Settings;

using Entity = VersionOne.VisualStudio.DataLayer.Entities.Entity;
using OmProject = VersionOne.SDK.ObjectModel.Project;
using Project = VersionOne.VisualStudio.DataLayer.Entities.Project;
using Workitem = VersionOne.VisualStudio.DataLayer.Entities.Workitem;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class DataLayerTester {
        private readonly IDataLayerInternal dataLayer = new ApiDataLayer();

        private string V1Url = TestConfiguration.GetVariable("V1Url");
        private string Username = TestConfiguration.GetVariable("V1User");
        private string Password = TestConfiguration.GetVariable("V1Pass");
        private const bool Integrated = false;

        private const string TestProjectName = "VS Integration tests project";
        private const string TestScheduleName = "VS Integration tests schedule";

        private const double EffortAmount = 3;
        private const double FloatingPointComparisonDelta = 0.001;

        #region V1 entities

        private V1Instance instance;
        private Member member;
        private Schedule schedule;
        private OmProject project;
        private Iteration iteration;
        private Story story1, story2;
        private Task task1, task2, task3;

        #endregion

        private static Story CreateStory(V1Instance instance, string name, OmProject project, Iteration iteration, Member owner) {
            var story = instance.Create.Story(name, project);

            if(owner != null) {
                story.Owners.Add(owner);
            }

            story.Iteration = iteration;
            story.Save();

            return story;
        }

        private static Task CreateTask(V1Instance instance, string name, PrimaryWorkitem parent, Member owner) {
            var task = instance.Create.Task(name, parent);

            task.Owners.Add(owner);
            task.Save();

            return task;
        }

        private void CreateTestData() {
            schedule = instance.Create.Schedule(TestScheduleName, new Duration(7, Duration.Unit.Days), new Duration(0, Duration.Unit.Days));

            member = instance.Create.Member("test user", "test");
            member.Save();
            
            project = instance.Create.Project(TestProjectName, AssetID.FromToken("Scope:0"), DateTime.Now.Date, schedule);

            iteration = instance.Create.Iteration(project);
            iteration.Activate();

            story1 = CreateStory(instance, "Story 1", project, iteration, instance.LoggedInMember);
            story1.Status.CurrentValue = "Future";
            story1.Save();

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.SecondaryWorkitem) {
                var story1Effort = story1.CreateEffort(EffortAmount);
                story1Effort.Save();
            }

            story2 = CreateStory(instance, "Story 2", project, iteration, member);

            task1 = CreateTask(instance, "Task 1", story1, instance.LoggedInMember);

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.PrimaryWorkitem) {
                var task1Effort = task1.CreateEffort(EffortAmount);
                task1Effort.Save();
            }

            task2 = CreateTask(instance, "Task 2", story1, member);
            task3 = CreateTask(instance, "Task 3", story2, member);
        }

        [TestFixtureSetUp]
        public void Before() {
            ServiceLocator.Instance.SetContainer(new StandardKernel());
            ServiceLocator.Instance.Container.Bind<IDataLayer>().ToConstant(dataLayer);
            ServiceLocator.Instance.Container.Bind<IDataLayerInternal>().ToConstant(dataLayer);

            instance = new V1Instance(V1Url, Username, Password, false);

            dataLayer.AddProperty("Name", Entity.StoryType, false);
            dataLayer.AddProperty("Name", Entity.ProjectType, false);
            dataLayer.AddProperty("BuildProjects", Entity.ProjectType, true);
            dataLayer.AddProperty("BuildProjects", Entity.ProjectType, false);
            dataLayer.AddProperty(Entity.ScheduleNameProperty, Entity.ProjectType, false);
            dataLayer.AddProperty("Owners", Entity.StoryType, false);
            dataLayer.AddProperty("Owners", Entity.TaskType, false);
            dataLayer.AddProperty("Owners", Entity.TestType, false);
            dataLayer.AddProperty("Owners", Entity.DefectType, false);
            dataLayer.AddProperty("Name", Entity.DefectType, false);
            dataLayer.AddProperty("Owners", Entity.StoryType, true);
            dataLayer.AddProperty("Owners.Nickname", Entity.StoryType, false);
            dataLayer.AddProperty(Entity.DoneProperty, Entity.StoryType, false);
            dataLayer.AddProperty(Entity.DoneProperty, Entity.TaskType, false);
            dataLayer.AddProperty(Entity.StatusProperty, Entity.StoryType, false);
            dataLayer.AddProperty(Entity.StatusProperty, Entity.StoryType, true);
            dataLayer.AddProperty(Entity.OrderProperty, Entity.StoryType, false);
            dataLayer.AddProperty(Entity.OrderProperty, Entity.TaskType, false);
            dataLayer.AddProperty(Entity.OrderProperty, Entity.TestType, false);
            dataLayer.AddProperty(Entity.OrderProperty, Entity.DefectType, false);
            dataLayer.AddProperty("Number", Entity.StoryType, false);
            dataLayer.AddProperty("Number", Entity.TaskType, false);
            dataLayer.AddProperty("Number", Entity.TestType, false);
            dataLayer.AddProperty("Number", Entity.DefectType, false);

            Assert.IsTrue(dataLayer.Connect(GetSettings()), "Connection validation");
            dataLayer.ShowAllTasks = true;

            CreateTestData();
        }

        private VersionOneSettings GetSettings() {
            return new VersionOneSettings {
                Path = V1Url,
                Username = Username,
                Password = Password,
                Integrated = Integrated
            };
        }

        [TestFixtureTearDown]
        public void TearDown() {
            var assetsToCleanup = new BaseAsset[]
                                  {task1, task2, task3, story1, story2, iteration, project, schedule, member};

            foreach(var asset in assetsToCleanup.Where(asset => asset != null && asset.CanDelete)) {
                asset.Delete();
            }
        }

        [Test]
        public void CheckConnection() {
            dataLayer.CheckConnection(GetSettings());
        }

        private Project FindTestProject(Project root, string name) {
            return root.Children.FirstOrDefault(child => child.GetProperty("Name").Equals(name) && child.Id.Equals(project.ID.Token));
        }

        [Test]
        public void GetProjects() {
            var projects = dataLayer.GetProjectTree();
            Assert.IsTrue(projects.Count > 0, "Projects exist");

            var testProject = FindTestProject(projects[0], TestProjectName);
            Assert.IsTrue(testProject != null, "Failed to retrieve test project");

            Assert.AreEqual(TestProjectName, testProject.GetProperty(Entity.NameProperty));
            Assert.AreEqual(TestScheduleName, testProject.GetProperty(Entity.ScheduleNameProperty));
        }

        [Test]
        public void SetCurrentProjectById() {
            var projects = dataLayer.GetProjectTree();
            var projectEnumerator = projects.GetEnumerator();
            Assert.IsTrue(projectEnumerator.MoveNext(), "Projects exist");

            dataLayer.CurrentProjectId = projectEnumerator.Current.Id;
            Assert.AreEqual(projectEnumerator.Current, dataLayer.CurrentProject);
            Assert.AreEqual(projectEnumerator.Current.GetProperty("Name"), dataLayer.CurrentProject.GetProperty("Name"));
        }

        [Test]
        //TODO doesn't work at suite run
        public void GetStories() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;

            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var stories = cache.GetWorkitems(dataLayer.ShowAllTasks);
            Assert.AreEqual(2, stories.Count);
            Assert.AreEqual(story1.ID.Token, stories[0].Id, "First story ID");
            Assert.AreEqual("Story 1", stories[0].GetProperty(Entity.NameProperty), "Story name");
            Assert.AreEqual(story1.Done, stories[0].GetProperty(Entity.DoneProperty), "Story efforts sum");
            Assert.AreEqual(2, stories[0].Children.Count, "First story children count");
            Assert.AreEqual(task1.ID.Token, stories[0].Children[0].Id, "First story first task ID");
            Assert.AreEqual(story2.ID.Token, stories[1].Id, "Second story ID");
            Assert.AreEqual(1, stories[1].Children.Count, "Second story children count");
            Assert.AreEqual(task3.ID.Token, stories[1].Children[0].Id, "Second story first task ID");
        }

        [Test]
        public void GetStoryListProperty() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;

            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var stories = cache.GetWorkitems(dataLayer.ShowAllTasks);
            Assert.IsTrue(stories.Count > 0);

            var status = stories[0].GetProperty(Entity.StatusProperty);
            Assert.AreEqual("Future", status.ToString());
        }

        [Test]
        public void GetEffort() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;
            
            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var stories = cache.GetWorkitems(dataLayer.ShowAllTasks);
            Assert.IsTrue(stories.Count > 0, "Stories exist");

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.SecondaryWorkitem) {
                Assert.AreEqual(EffortAmount, (double) stories[0].GetProperty(Entity.DoneProperty),
                                FloatingPointComparisonDelta);
                Assert.AreEqual(stories[0].GetProperty(Entity.EffortProperty), null);
            }

            Assert.IsTrue(stories[0].Children.Count > 0, "Tasks exist");

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.PrimaryWorkitem) {
                Assert.AreEqual(EffortAmount, (double)stories[0].Children[0].GetProperty(Entity.DoneProperty),
                                FloatingPointComparisonDelta);
                Assert.AreEqual(stories[0].Children[0].GetProperty(Entity.EffortProperty), null);
            }
        }

        [Test]
        public void SetEffort() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;
            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);

            var stories = cache.GetWorkitems(dataLayer.ShowAllTasks);
            Assert.AreEqual(null, stories[0].GetProperty(Entity.EffortProperty), "Story effort");

            stories[0].SetProperty(Entity.EffortProperty, 5.25);
            Assert.AreEqual(5.25, (double) stories[0].GetProperty(Entity.EffortProperty), FloatingPointComparisonDelta);
            Assert.IsTrue(stories[0].Children.Count > 0, "Tasks exist");
            Assert.AreEqual(null, stories[0].Children[0].GetProperty(Entity.EffortProperty), "Task effort");
            
            stories[0].Children[0].SetProperty(Entity.EffortProperty, 1.75);
            Assert.AreEqual(1.75, (double) stories[0].Children[0].GetProperty(Entity.EffortProperty),
                            FloatingPointComparisonDelta);
        }

        [Test]
        public void CommitEffort() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;
            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var stories = cache.GetWorkitems(dataLayer.ShowAllTasks);
            var workitemForRefresh = new List<Workitem>();

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.SecondaryWorkitem) {
                var story = stories[1];
                Assert.AreEqual(null, story.GetProperty(Entity.EffortProperty), "Story effort");

                story.SetProperty(Entity.EffortProperty, 5.25);
                Assert.AreEqual(5.25, (double)story.GetProperty(Entity.EffortProperty), FloatingPointComparisonDelta);
                workitemForRefresh.Add(story);
            }

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.PrimaryWorkitem) {
                var task = stories[1].Children[0];
                Assert.AreEqual(null, task.GetProperty(Entity.EffortProperty), "Task effort");

                task.SetProperty(Entity.EffortProperty, 1.75);
                Assert.AreEqual(1.75, task.GetProperty(Entity.EffortProperty), "Task new effort");
                workitemForRefresh.Add(task);
            }

            var storyDone = Convert.ToDouble(stories[1].GetProperty(Entity.DoneProperty));
            var taskDone = Convert.ToDouble(stories[1].Children[0].GetProperty(Entity.DoneProperty));

            dataLayer.CommitChanges(cache);
            cache.Drop();// look at WorkitemTreeController.HandleSaveCommand
            projects = dataLayer.GetProjectTree();
            testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;
            dataLayer.GetWorkitems(cache);
            stories = cache.GetWorkitems(dataLayer.ShowAllTasks);

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.SecondaryWorkitem) {
                Assert.AreEqual(storyDone + 5.25, (double) stories[1].GetProperty(Entity.DoneProperty), FloatingPointComparisonDelta);
            }

            if (dataLayer.EffortTracking.StoryTrackingLevel != EffortTrackingLevel.PrimaryWorkitem) {
                Assert.AreEqual(taskDone + 1.75, (double)stories[1].Children[0].GetProperty(Entity.DoneProperty), FloatingPointComparisonDelta);
            }
        }

        [Test]
        //TODO doesn't work at suite run
        public void ReadOnlyAssetsForCurrentUser() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            dataLayer.CurrentProject = testProject;
            
            dataLayer.ShowAllTasks = false;

            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var workitems = cache.GetWorkitems(dataLayer.ShowAllTasks);
            Assert.AreEqual(1, workitems.Count);

            var workitem = GetWorkitemByName("Story 1");

            Assert.IsNotNull(workitem);
            Assert.AreEqual(1, workitem.Children.Count);

            workitem = GetWorkitemByName("Story 2");
            Assert.IsNull(workitem);

            // show all tasks, stories, defects and tests
            dataLayer.ShowAllTasks = true;
            Assert.AreEqual(2, cache.GetWorkitems(dataLayer.ShowAllTasks).Count);

            workitem = GetWorkitemByName("Story 1");
            Assert.IsNotNull(workitem);
            Assert.AreEqual(2, workitem.Children.Count);

            workitem = GetWorkitemByName("Story 2");
            Assert.IsNotNull(workitem);
            Assert.AreEqual(1, workitem.Children.Count);
        }

        [Test]
        public void ChildrenSorting() {
            var projects = dataLayer.GetProjectTree();
            var testProject = FindTestProject(projects[0], TestProjectName);
            var omProject = GetOmProject(TestProjectName, testProject.Id);
            dataLayer.CurrentProject = testProject;

            var v1Instance = new V1Instance(V1Url, Username, Password, false);
            Story story = null;

            try {
                story = CreateStory(v1Instance, "New Story with ordered children", omProject, iteration, v1Instance.LoggedInMember);
                var firstTask = story.CreateTask("Task1");
                var secondTask = story.CreateTask("Task2");
                var thirdTask = story.CreateTask("Task3");

                RankAboveAll(thirdTask, firstTask);
                RankAboveAll(secondTask, firstTask, thirdTask);

                dataLayer.Connect(GetSettings());

                var cache = dataLayer.CreateAssetCache();
                dataLayer.GetWorkitems(cache);
                var primaryWorkitems = cache.GetWorkitems(dataLayer.ShowAllTasks);
                var foundStory = primaryWorkitems.FirstOrDefault(workitem => workitem.GetProperty(Entity.NameProperty).Equals(story.Name) && workitem.GetProperty("Number").Equals(story.DisplayID));

                Assert.IsNotNull(foundStory);

                AssertWorkitemMatch(foundStory.Children[0], secondTask.Name, Entity.TaskType);
                AssertWorkitemMatch(foundStory.Children[1], thirdTask.Name, Entity.TaskType);
                AssertWorkitemMatch(foundStory.Children[2], firstTask.Name, Entity.TaskType);
            } finally {
                if (story != null) {
                    story.Delete();
                }
            }
        }

        private Workitem GetWorkitemByName(string name) {
            var cache = dataLayer.CreateAssetCache();
            dataLayer.GetWorkitems(cache);
            var workitems = cache.GetWorkitems(dataLayer.ShowAllTasks);
            return workitems.FirstOrDefault(item => item.GetProperty("Name").ToString() == name);
        }

        private OmProject GetOmProject(string name, string token) {
            var filter = new ProjectFilter();
            filter.Name.Add(name);
            var projects = instance.Get.Projects(filter);

            return projects.FirstOrDefault(item => item.ID.Token.Equals(token));
        }

        private static void RankAboveAll(Task first, params Task[] other) {
            foreach(var task in other) {
                first.RankOrder.SetAbove(task);
            }
        }

        private static void AssertWorkitemMatch(Workitem item, string name, string type) {
            Assert.AreEqual(item.GetProperty(Entity.NameProperty), name);
            Assert.AreEqual(item.TypePrefix, type);
        }
    }
}