using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    [TestFixture(Category = "Exception Tests")]
    public class ConstructionExceptionTests
    {
        [Test]
        public void EmptyTargetThrowsError()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Notifications", "Changed" },
            };
            Inflator inflator = new Inflator();

            Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, false));
        }

        [Test]
        public void EmptyNotificationsThrowsError()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "C:/test" },
            };
            Inflator inflator = new Inflator();

            Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, false));
        }
    }
}