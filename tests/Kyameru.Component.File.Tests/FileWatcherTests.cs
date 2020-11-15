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
            this.location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [Test]
        public void RenameWorks()
        {
            this.CheckFile("rename.tdd");
            this.CheckFile("renamed.tdd");
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            string method = string.Empty;
            System.IO.File.WriteAllText($"{this.location}/rename.tdd", "test data");
            FileWatcher from = this.Setup("Renamed");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.Move($"{this.location}/rename.tdd", $"{this.location}/renamed.tdd");
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.AreEqual("Renamed", method);
        }

        [Test]
        public void CreatedWorks()
        {
            this.CheckFile("created.tdd");
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            string method = string.Empty;

            FileWatcher from = this.Setup("Created");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "test data");
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.AreEqual("Created", method);
        }

        private void From_OnAction(object sender, Core.Entities.Routable e)
        {
            this.message = e;
        }

        public File.FileWatcher Setup(string notification)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", this.location },
                { "Notifications", notification },
                { "Filter", "*.*" },
                { "SubDirectories", "true" }
            };

            return new FileWatcher(headers);
        }

        private void CheckFile(string file)
        {
            if (System.IO.File.Exists($"{location}/{file}"))
            {
                System.IO.File.Delete($"{location}/{file}");
            }
        }
    }
}