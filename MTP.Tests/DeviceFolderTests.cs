using System;
using NUnit.Framework;

namespace Mtp.Tests
{
    [TestFixture]
    public class DeviceFolderTests
    {
        private DeviceFolder _root;

        [SetUp]
        public void Setup()
        {
            _root = new DeviceFolder
            {
                Id = "1",
                Name = "a device"
            };

            var secondLevel = new DeviceFolder
            {
                Id = "2",
                Name = "2nd level"
            };

            var thirdLevel = new DeviceFolder
            {
                Id = "3",
                Name = "3rd level",
            };

            var fourthLevel = new DeviceFolder
            {
                Id = "4",
                Name = "3rd level"
            };
            _root.AddChild(secondLevel);
            secondLevel.AddChild(thirdLevel);
            thirdLevel.AddChild(fourthLevel);
        }

        [Test]
        public void FindDeepestMatch_returns_last_folder_object_if_path_has_exact_match()
        {
            var output = _root.FindDeepestMatch(new[] { "a device", "2nd level", "3rd level" });

            Assert.That(output.Id, Is.EqualTo("3"));
            Assert.That(output.Depth, Is.EqualTo(2));
        }

        [Test]
        public void FindDeepestMatch_returns_last_matching_folder_object_if_path_has_partial_match()
        {
            var output = _root.FindDeepestMatch(new[] { "a device", "2nd level" });

            Assert.That(output.Id, Is.EqualTo("2"));
            Assert.That(output.Depth, Is.EqualTo(1));
        }

        [Test]
        public void FindDeepestMatch_throws_ArgumentException_if_start_of_path_doesnt_match()
        {
            TestDelegate testAction = () => _root.FindDeepestMatch(new[] { "another device", "2nd level", "3rd level" });

            Assert.Throws<ArgumentException>(testAction);
        }

        [Test]
        public void FindDeepestMatch_handles_duplicate_path_level_names()
        {
            var output = _root.FindDeepestMatch(new[] { "a device", "2nd level", "3rd level", "3rd level" });

            Assert.That(output.Id, Is.EqualTo("4"));
            Assert.That(output.Depth, Is.EqualTo(3));

        }
    }
}