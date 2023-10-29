using Calendar.shared;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Shared.Models.WebApi.Requests
{
    public class EventCreateRequest
    {
        [Required]
        [MaxLength(Constants.MaxLength.NAME)]
        public string Title { get; set; }

        // TODO make this not required
        [Required]
        [MaxLength(Constants.MaxLength.DESCRIPTION)]
        public string Description { get; set; }

        [Required]
        public bool AllDay { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
