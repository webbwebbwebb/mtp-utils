using System.IO;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ParseDeviceName_extracts_text_between_first_and_second_path_separators_if_string_begins_with_path_separator()
        {
            var input = Path.DirectorySeparatorChar + "device name" + Path.DirectorySeparatorChar + "storage";

            var output = input.ParseDeviceName();

            Assert.That(output, Is.EqualTo("device name"));
        }

        [Test]
        public void ParseDeviceName_extracts_text_up_to_first_path_separator_if_string_does_not_begin_with_path_separator()
        {
            var input = "device name" + Path.DirectorySeparatorChar + "storage";

            var output = input.ParseDeviceName();

            Assert.That(output, Is.EqualTo("device name"));
        }

        [Test]
        public void ParseFilePath_extracts_text_before_last_path_separator()
        {
            var input = @"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar + "*.*";

            var output = input.ParseFilePath();

            Assert.That(output, Is.EqualTo(@"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar));
        }

        [Test] 
        public void ParseFilePath_returns_all_text_if_path_separator_is_the_last_character()
        {
            var input = @"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar;

            var output = input.ParseFilePath();

            Assert.That(output, Is.EqualTo(@"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar));
        }

        [Test]
        public void ParseSearchPattern_extracts_text_after_last_path_separator()
        {
            var input = @"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar + "*.*";

            var output = input.ParseSearchPattern();

            Assert.That(output, Is.EqualTo("*.*"));
        }

        [Test]
        public void ParseSearchPattern_returns_empty_string_if_path_separator_is_the_last_character()
        {
            var input = @"C:" + Path.DirectorySeparatorChar + "temp" + Path.DirectorySeparatorChar;

            var output = input.ParseSearchPattern();

            Assert.That(output, Is.EqualTo(string.Empty));
        }
    }
}
