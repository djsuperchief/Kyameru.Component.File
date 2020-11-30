using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class FileToTests
    {
        private readonly string fileLocation;

        public FileToTests()
        {
            this.fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/") + "/test";
        }

        [Test]
        [TestCase("Move")]
        [TestCase("Copy")]
        [TestCase("Write")]
        public void CanDoAction(string action)
        {
            string randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
            FileTo fileTo = this.Setup(action, randomFileName);

            Dictionary<string, string> routableHeaders = new Dictionary<string, string>()
            {
                { "FullSource", $"test/{randomFileName}" },
                { "SourceFile", randomFileName }
            };
            fileTo.Process(new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file")));
            Assert.IsTrue(System.IO.File.Exists($"{this.fileLocation}/target/{randomFileName}"));
        }

        [Test]
        public void CanDeleteFile()
        {
            string randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
            FileTo fileTo = this.Setup("Delete", randomFileName);
            Dictionary<string, string> routableHeaders = new Dictionary<string, string>()
            {
                { "FullSource", $"test/{randomFileName}" },
                { "SourceFile", randomFileName }
            };
            fileTo.Process(new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file")));
            Assert.IsFalse(System.IO.File.Exists($"test/{randomFileName}"));
        }

        private FileTo Setup(string action, string randomFileName)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            System.IO.File.WriteAllText($"{fileLocation}/{randomFileName}", "test file");
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", $"{this.fileLocation}/target" },
                { "Action", action }
            };

            return new FileTo(headers);
        }
    }
}