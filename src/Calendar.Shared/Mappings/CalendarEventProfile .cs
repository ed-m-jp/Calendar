using AutoMapper;
using Calendar.shared.Entities;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Response;

namespace Calendar.Shared.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            // Map from Entity to DTO
            CreateMap<Event, EventResponse>();
            CreateMap<Event, PartialEventResponse>();
            CreateMap<Event, EventUpdateRequest>();

            // Map from DTO to Entity
            CreateMap<EventCreateRequest, Event> ();
            CreateMap<EventUpdateRequest, Event>();
        }
    }
}
