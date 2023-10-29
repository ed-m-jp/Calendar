using AutoMapper;
using Calendar.DataAccess.Interfaces;
using Calendar.Services.Interfaces;
using Calendar.shared.Entities;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Response;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository       _EventRepository;
        private readonly ILogger<EventService>  _logger;
        private readonly IMapper                _mapper;

        public EventService(IEventRepository EventRepository,
                            ILogger<EventService> logger,
                            IMapper mapper)
        {
            _EventRepository = EventRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ServiceResult<EventResponse>> GetEventbyIdAsync([NotNull] int eventId)
        {
            var getResult = await _EventRepository.GetByIdAsync(eventId);
            return ServiceResult<EventResponse>
                .FromRepositoryActionResult(getResult,
                    () => _mapper.Map<EventResponse>(getResult.Entity),
                    $"An error happened trying to get Event for event id : [{eventId}]");
        }

        public async Task<ServiceResult<EventResponse>> AddEventAsync([NotNull] EventCreateRequest createRequest, [NotNull] string userId)
        {
            (createRequest.StartTime, createRequest.EndTime)
                = NormalizeStartAndEndTimes(createRequest.AllDay, createRequest.StartTime, createRequest.EndTime);

            if (IsStartDateAfterOrEqualToEndDate(createRequest.StartTime, createRequest.EndTime) == true)
            {
                return ServiceResult<EventResponse>.Unprocessable("Start time should be before end time.");
            }

            var newEventEntity = _mapper.Map<Event>(createRequest);
            newEventEntity.UserId = userId!;

            var saveResult = await _EventRepository.AddAsync(newEventEntity);
            if (saveResult.IsError)
                return ServiceResult<EventResponse>.Error(saveResult.ErrorMessage);

            return ServiceResult<EventResponse>.Ok(_mapper.Map<EventResponse>(newEventEntity));
        }

        public async Task<ServiceResult> DeleteEventAsync([NotNull] int eventId)
        {
            var deleteResult = await _EventRepository.DeleteAsync(eventId);
            return ServiceResult.FromRepositoryActionResult(deleteResult, $"Failed to delete event for Event id: [{eventId}]");
        }

        public async Task<ServiceResult<EventResponse>> PartialUpdateEventAsync([NotNull] int eventId,
            [NotNull] JsonPatchDocument<EventUpdateRequest> patchDoc)
        {
            var eventFromDb = await _EventRepository.GetByIdAsync(eventId);

            if (eventFromDb.IsNotFound)
                return ServiceResult<EventResponse>.NotFound();
            if (eventFromDb.IsError)
                return ServiceResult<EventResponse>.Error($"Failed to Update Event for event id : [{eventId}]");

            try
            {
                // Convert the database entity to a DTO which matches the shape of the patch document
                var eventToUpdateDto = _mapper.Map<EventUpdateRequest>(eventFromDb.Entity);

                // Apply the JSON Patch
                patchDoc.ApplyTo(eventToUpdateDto);

                // Now map the DTO back to the entity
                _mapper.Map(eventToUpdateDto, eventFromDb.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error partially updating the calendar event.");
                return ServiceResult<EventResponse>.Unprocessable();
            }

            (eventFromDb.Entity!.StartTime, eventFromDb.Entity!.EndTime)
                = NormalizeStartAndEndTimes(eventFromDb.Entity!.AllDay, eventFromDb.Entity!.StartTime, eventFromDb.Entity!.EndTime);

            if (IsStartDateAfterOrEqualToEndDate(eventFromDb.Entity!.StartTime, eventFromDb.Entity!.EndTime) == true)
            {
                return ServiceResult<EventResponse>.Unprocessable("Start time should be before end time.");
            }

            // Save the updated entity to the DB
            var saveResult = await _EventRepository.UpdateAsync(eventFromDb.Entity!);

            if (saveResult.IsError)
                return ServiceResult<EventResponse>.Error($"Failed to partially update Event for event id : [{eventId}].");

            return ServiceResult<EventResponse>.Ok(_mapper.Map<EventResponse>(eventFromDb.Entity));
        }

        public async Task<ServiceResult<EventResponse>> UpdateEventAsync([NotNull] int eventId,
            [NotNull] EventUpdateRequest updateRequest)
        {
            var eventFromDb = await _EventRepository.GetByIdAsync(eventId);

            if (eventFromDb.IsNotFound)
                return ServiceResult<EventResponse>.NotFound();
            if (eventFromDb.IsError)
                return ServiceResult<EventResponse>.Error($"Failed to Update Event for event id : [{eventId}].");

            (updateRequest.StartTime, updateRequest.EndTime)
                    = NormalizeStartAndEndTimes(eventFromDb.Entity!.AllDay, updateRequest.StartTime, updateRequest.EndTime);

            if (IsStartDateAfterOrEqualToEndDate(updateRequest.StartTime, updateRequest.EndTime) == true)
            {
                return ServiceResult<EventResponse>.Unprocessable("Start time should be before end time.");
            }

            try
            {
                // update all the entity values
                _mapper.Map(updateRequest, eventFromDb.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the calendar event.");
                return ServiceResult<EventResponse>.Unprocessable();
            }

            // Save the updated entity to the DB
            var saveResult = await _EventRepository.UpdateAsync(eventFromDb.Entity!);

            if (saveResult.IsError)
                return ServiceResult<EventResponse>.Error($"Failed to Update Event for event id : [{eventId}].");

            return ServiceResult<EventResponse>.Ok(_mapper.Map<EventResponse>(eventFromDb.Entity));
        }

        public async Task<ServiceResult<IReadOnlyList<PartialEventResponse>>> GetEventListForCurrentUserOnDate(
            [NotNull] DateTime date, [NotNull] string userId)
        {
            var getResult = await _EventRepository.GetEventsForUserOnDateAsync(userId!, date);

            return ServiceResult<IReadOnlyList<PartialEventResponse>>.FromRepositoryActionResult(getResult,
                () => _mapper.Map<IReadOnlyList<PartialEventResponse>>(getResult.Entity));
        }

        public async Task<ServiceResult<IReadOnlyList<PartialEventResponse>>> GetEventListForCurrentUserBetweenDates(
            [NotNull] DateTime startDate, [NotNull] DateTime endDate, [NotNull] string userId)
        {
            if (startDate.Date > endDate.Date)
            {
                return ServiceResult<IReadOnlyList<PartialEventResponse>>.BadRequest("start date must be inferior or equal to end date.");
            }

            var getResult = await _EventRepository.GetEventsForUserBetweenDatesAsync(userId!, startDate, endDate);

            return ServiceResult<IReadOnlyList<PartialEventResponse>>.FromRepositoryActionResult(getResult,
                () => _mapper.Map<IReadOnlyList<PartialEventResponse>>(getResult.Entity));
        }

        private static (DateTime, DateTime) NormalizeStartAndEndTimes(bool allDay, DateTime startTime, DateTime endTime)
        {
            if (allDay == true)
            {
                startTime = startTime.Date;
                endTime = endTime.Date;
            }

            return (startTime, endTime);
        }

        private static bool IsStartDateAfterOrEqualToEndDate(DateTime startDate, DateTime endDate)
            => startDate >= endDate;
    }
}
