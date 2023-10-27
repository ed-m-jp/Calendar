using System.Text;

namespace Calendar.ServiceLayer.Services
{
    public static class EncoderHelper
    {

        private static readonly byte[] _salt = Encoding.UTF8.GetBytes("hJuVo cFg2B3 SDf7pa");

        public static string Encode(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= _salt[i % _salt.Length];
                bytes[i] = (byte)(bytes[i] << 3 | bytes[i] >> 5);
            }

            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string encodedData)
        {
            var bytes = Convert.FromBase64String(encodedData);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] >> 3 | bytes[i] << 5);
                bytes[i] ^= _salt[i % _salt.Length];
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
