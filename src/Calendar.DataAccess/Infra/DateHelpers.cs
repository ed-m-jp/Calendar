using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Calendar.DataAccess.Infra
{
    internal static class DateHelpers
    {
        internal static ValueConverter UTCNormalizer()
        {
            Expression<Func<DateTime, DateTime>> normalizeDate = d => d.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : d.ToUniversalTime();
            
            return new ValueConverter<DateTime, DateTime>(normalizeDate, normalizeDate);
        }
    }
}
