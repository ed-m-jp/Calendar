using Calendar.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar.shared.Entities
{
    [Table("Events")]
    public class Event: EntityBase
    {
        [Required]
        [MaxLength(Constants.MaxLength.NAME)]
        public string Title { get; set; }

        [Required]
        [MaxLength(Constants.MaxLength.DESCRIPTION)]
        public string Description { get; set; }

        [Required]
        public bool AllDay { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
