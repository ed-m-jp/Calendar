using Calendar.DataAccess.Infra;
using Calendar.shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.DataAccess.EntityConfiguration
{
    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> Event)
        {
            // Properties
            //Event.Property(e => e.StartTime).HasConversion(DateHelpers.UTCNormalizer());
            //Event.Property(e => e.EndTime).HasConversion(DateHelpers.UTCNormalizer());

            // Indexes
            Event.HasIndex(x => new { x.StartTime, x.EndTime }).IsUnique(false);
        }
    }
}
