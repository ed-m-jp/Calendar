using Calendar.shared;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Shared.Models.WebApi.Requests
{
    public class EventUpdateRequest
    {

        [MaxLength(Constants.MaxLength.NAME)]
        public string Title { get; set; }

        [MaxLength(Constants.MaxLength.DESCRIPTION)]
        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
