using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class InflatorTests
    {
        [Test]
        public void CanGetFrom()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/" },
                { "Notifications", "Created" },
                { "Filter", "*.tdd" },
                { "SubDirectories", "true" }
            };
            Inflator inflator = new Inflator();
            Assert.NotNull(inflator.CreateFromComponent(headers));
        }

        [Test]
        public void CanGetTo()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/target" },
                { "Action", "Move" },
                { "Overwrite","true" }
            };

            Inflator inflator = new Inflator();
            Assert.NotNull(inflator.CreateToComponent(headers));
        }

        [Test]
        public void AtomicThrows()
        {
            Inflator inflator = new Inflator();
            Assert.Throws<NotImplementedException>(() => inflator.CreateAtomicComponent(null));
        }
    }
}