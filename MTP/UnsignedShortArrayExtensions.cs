using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApplication1
{
    public static class UnsignedShortArrayExtensions
    {
        public static byte[] ToByteArray(this ushort[] data)
        {
            var asBytes = new byte[data.Length * sizeof(ushort)];
            Buffer.BlockCopy(data, 0, asBytes, 0, asBytes.Length);

            return asBytes;
        }

        public static string ConvertToString(this ushort[] data)
        {
            return Encoding.Unicode.GetString(data.ToByteArray());
        }
    }
}