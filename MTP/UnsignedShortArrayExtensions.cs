using System;
using System.Text;

namespace Mtp
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