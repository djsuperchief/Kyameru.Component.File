using Kyameru.Core.Entities;
using Moq;
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
        private readonly Mock<Component.File.Utilities.IFileSystemWatcher> fileSystemWatcher = new Mock<Utilities.IFileSystemWatcher>();
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
            FileWatcher from = this.Setup("Created");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "test data");
            this.fileSystemWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, this.location, "Created.tdd"));
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            from.Stop();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(method));
        }

        [Test]
        public void ChangedWorks()
        {
            string filename = $"{Guid.NewGuid().ToString("N")}.txt";
            this.CheckFile(filename);
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            System.IO.File.WriteAllText($"{this.location}/{filename}", "test data");
            string method = string.Empty;

            FileWatcher from = this.Setup("Changed");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };
            from.Setup();
            from.Start();
            System.IO.File.WriteAllText($"{this.location}/{filename}", "more data added");
            System.IO.File.WriteAllText($"{this.location}/{filename}", "more data added");
            this.fileSystemWatcher.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, this.location, filename));
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.AreEqual("Changed", method);
        }

        public File.FileWatcher Setup(string notification)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/" },
                { "Notifications", notification },
                { "SubDirectories", "true" }
            };

            return new FileWatcher(headers, this.fileSystemWatcher.Object);
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