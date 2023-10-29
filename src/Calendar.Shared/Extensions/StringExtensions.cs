using Calendar.ServiceLayer.Services;

namespace Calendar.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToPublicId(this string userId)
            => EncoderHelper.Encode(userId);

        public static string ToInternalId(this string encodedUserId)
            => EncoderHelper.Decode(encodedUserId);
    }
}
