using System.IO;

namespace Mtp
{
    public static class StringExtensions
    {
        public static string ParseDeviceName(this string input)
        {
            var deviceName = input.TrimStart(Path.DirectorySeparatorChar);
            deviceName = deviceName.Substring(0, deviceName.IndexOf(Path.DirectorySeparatorChar));
            return deviceName;
        }

        public static string ParseSearchPattern(this string input)
        {
            var index = input.LastIndexOf(Path.DirectorySeparatorChar);
            if (index > -1)
            {
                return input.Substring(index + 1);
            }

            return string.Empty;
        }

        public static string ParseFilePath(this string input)
        {
            var index = input.LastIndexOf(Path.DirectorySeparatorChar);
            if (index > -1)
            {
                return input.Substring(0, index + 1);
            }

            return input;
        }

        public static string TrimNullTermination(this string input)
        {
            return input.Trim('\0');
        }
    }
}