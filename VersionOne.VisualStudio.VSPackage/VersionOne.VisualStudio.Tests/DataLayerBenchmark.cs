using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using NUnit.Framework;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Settings;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    //this is not unit test but benchmark.
    public class DataLayerBenchmark {
        private const int Tries = 10;
        private readonly VersionOneSettings settings;
        private IDataLayer apiDataLayer;

        public DataLayerBenchmark() {
            settings = new VersionOneSettings {
                Path = "https://www3.v1host.com/ExigenTest/",
                Username = "admin",
                Password = "admin",
                Integrated = false,
                ProxySettings = { UseProxy = false }
            };            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup() {
            const string fileName = "configuration.xml";
            var fullPath = Assembly.GetAssembly(typeof(TaskWindow)).Location;
            var dirName = fullPath.Substring(0, fullPath.LastIndexOf("\\") + 1);
            File.Copy("./" + fileName, dirName + "/" + fileName);
        }

        [SetUp]        
        public void SetUp() {
            var cfg = Configuration.Instance;
            WeakUpServer();
            apiDataLayer = ApiDataLayer.Instance;
            AddProperties(cfg);

        }         

        [Test]
        public void Connect() {
            // 31-meta 16-data
            Console.WriteLine("Test Connect");
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < Tries; i++) {
                var innerTimer = Stopwatch.StartNew();
                apiDataLayer.Connect(settings);
                Console.WriteLine((i + 1) + " try:" + (innerTimer.Elapsed.TotalMilliseconds / 1000).ToString("0.00 sec") + "( " + (innerTimer.Elapsed.TotalMilliseconds).ToString("0.00 ms") + " )");
            }
            Console.WriteLine(((timer.Elapsed.TotalMilliseconds / Tries) / 1000).ToString("0.00 sec") + "( " + (timer.Elapsed.TotalMilliseconds / Tries).ToString("0.00 ms") + " )");
        }


        private void WeakUpServer() {
            ApiDataLayer.Instance.Connect(settings);
        }

        private static void AddProperties(Configuration cfg) {
            LoadOrderProperties();

            foreach (var column in cfg.AssetDetail.TaskColumns) {
                AddProperty(column, Entity.TaskPrefix);
            }

            foreach (var column in cfg.AssetDetail.StoryColumns) {
                AddProperty(column, Entity.StoryPrefix);
            }

            foreach (var column in cfg.AssetDetail.DefectColumns) {
                AddProperty(column, Entity.DefectPrefix);
            }

            foreach (var column in cfg.AssetDetail.TestColumns) {
                AddProperty(column, Entity.TestPrefix);
            }

            foreach (var column in cfg.GridSettings.Columns) {
                AddProperty(column, Entity.TaskPrefix);
                AddProperty(column, Entity.StoryPrefix);
                AddProperty(column, Entity.DefectPrefix);
                AddProperty(column, Entity.TestPrefix);
            }

            foreach (var column in cfg.ProjectTree.Columns) {
                AddProperty(column, Entity.ProjectPrefix);
            }
        }

        /// <summary>
        /// We do not support Order property on UI. That's why configuration.xml is not used.
        /// Order property has special type, it currently can not be set properly.
        /// </summary>
        private static void LoadOrderProperties() {
            ApiDataLayer.Instance.AddProperty(Entity.OrderProperty, Entity.TestPrefix, false);
            ApiDataLayer.Instance.AddProperty(Entity.OrderProperty, Entity.TaskPrefix, false);
        }

        private static void AddProperty(ColumnSetting column, string prefix) {
            ApiDataLayer.Instance.AddProperty(column.Attribute, prefix, column.Type == "List" || column.Type == "Multi");
            //ApiDataLayer.Instance.AddProperty(column.Attribute, prefix, false);
        }
    }
}