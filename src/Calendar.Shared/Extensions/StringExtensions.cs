using Calendar.ServiceLayer.Services;

namespace Calendar.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToPublicId(this string userId)
        {
            return EncoderHelper.Encode(userId);
        }

        public static string ToInternalId(this string encodedUserId)
        {
            return EncoderHelper.Decode(encodedUserId);
        }
    }
}
