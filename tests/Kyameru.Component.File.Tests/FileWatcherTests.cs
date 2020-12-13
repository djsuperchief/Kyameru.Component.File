using Kyameru.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class FileWatcherTests
    {
        private readonly string location;
        private Routable message;

        public FileWatcherTests()
        {
            this.location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/test";
        }

        [Test]
        public void CreatedWorks()
        {
            this.CheckFile("created.tdd");
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            string method = string.Empty;

            // Github tests for some reason do not raise created compared to local os.
            FileWatcher from = this.Setup("Created,Changed");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "test data");
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(method));
        }

        [Test]
        public void ChangedWorks()
        {
            this.CheckFile("created.tdd");
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "test data");
            string method = string.Empty;

            FileWatcher from = this.Setup("Changed");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "more data added");
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.AreEqual("Changed", method);
        }

        private void From_OnAction(object sender, Core.Entities.Routable e)
        {
            this.message = e;
        }

        public File.FileWatcher Setup(string notification)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/" },
                { "Notifications", notification },
                { "SubDirectories", "true" }
            };

            return new FileWatcher(headers);
        }

        private void CheckFile(string file)
        {
            if (!Directory.Exists(this.location))
            {
                Directory.CreateDirectory(this.location);
            }

            if (System.IO.File.Exists($"{location}/{file}"))
            {
                System.IO.File.Delete($"{location}/{file}");
            }
        }
    }
}