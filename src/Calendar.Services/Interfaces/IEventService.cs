using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Response;
using Microsoft.AspNetCore.JsonPatch;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.ServiceLayer.Interfaces
{
    public interface IEventService
    {
        Task<ServiceResult<EventResponse>> AddEventAsync([NotNull] EventCreateRequest createRequest, [NotNull] string userId);

        Task<ServiceResult> DeleteEventAsync([NotNull] int eventId);

        Task<ServiceResult<EventResponse>> GetEventbyIdAsync([NotNull] int eventId);

        Task<ServiceResult<EventResponse>> PartialUpdateEventAsync([NotNull] int eventId, [NotNull] JsonPatchDocument<EventUpdateRequest> patchDoc);
        
        Task<ServiceResult<EventResponse>> UpdateEventAsync([NotNull] int eventId, [NotNull] EventUpdateRequest updateRequest);

        Task<ServiceResult<IReadOnlyList<PartialEventResponse>>> GetEventListForCurrentUserOnDate([NotNull] DateTime date, [NotNull] string userId);

        Task<ServiceResult<IReadOnlyList<PartialEventResponse>>> GetEventListForCurrentUserBetweenDates([NotNull] DateTime startDate, [NotNull] DateTime endDate, [NotNull] string userId);
    }
}