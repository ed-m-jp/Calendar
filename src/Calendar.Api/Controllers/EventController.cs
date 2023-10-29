using Calendar.Services.Interfaces;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class EventController : ApiControllerBase
    {
        private readonly IEventService              _EventService;
        private readonly ILogger<EventController>   _logger;

        public EventController(IEventService EventService, ILogger<EventController> logger)
        {
            _EventService = EventService;
            _logger = logger;
        }

        /// <summary>
        /// Get a calendar event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(void))]
        [HttpGet("{eventId}", Name = "getEvent")]
        public async Task<ActionResult<EventResponse>> GetEvent([FromRoute] int eventId)
        {
            var getResult = await _EventService.GetEventbyIdAsync(eventId);

            if (getResult.IsOk)
                return getResult.Data!;
            else if (getResult.IsNotFound)
                return NotFound();

            throw new ApplicationException(getResult.ErrorMessage);
        }

        /// <summary>
        /// Add new calendar event.
        /// </summary>
        /// <param name="createRequest"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EventResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Consumes("application/json")]
        [HttpPost(Name = "createEvent")]
        public async Task<ActionResult<EventResponse>> AddEvent([FromBody] EventCreateRequest createRequest)
        {
            var addResult = await _EventService.AddEventAsync(createRequest, UserId!);

            if (addResult.IsOk)
                return CreatedAtRoute(nameof(GetEvent), new { version = "1", eventId = addResult.Data!.Id }, addResult.Data);
            else if (addResult.IsUnprocessable)
                return UnprocessableEntity(addResult.ErrorMessage);

            throw new ApplicationException(addResult.ErrorMessage);
        }

        /// <summary>
        /// Delete a calendar event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{eventId}", Name = "deleteEvent")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int eventId)
        {
            var deleteResult = await _EventService.DeleteEventAsync(eventId);

            if (deleteResult.IsOk)
                return NoContent();
            else if (deleteResult.IsNotFound)
                return NotFound();

            throw new ApplicationException(deleteResult.ErrorMessage);
        }

        /// <summary>
        /// Update calendar event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPatch("{eventId}")]
        public async Task<ActionResult<EventResponse>> UpdateEvent([FromRoute] int eventId, [FromBody] EventUpdateRequest updateRequest)
        {
            var updateResult = await _EventService.UpdateEventAsync(eventId, updateRequest);

            if (updateResult.IsUnprocessable)
                return UnprocessableEntity(updateResult.ErrorMessage);

            return Ok(updateResult.Data);
        }

        /// <summary>
        /// Update Calendar Event Partially.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="jsonPatch"></param>
        /// <returns></returns>
        /// /// <remarks>
        /// Use JSON Patch to send operations that modify specific fields of a calendar event.
        /// 
        /// Available fields to update:
        /// - /title
        /// - /description
        /// - /startTime
        /// - /endTime
        /// 
        /// Sample request:
        /// 
        ///     PATCH /{eventId}/partial
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/title",
        ///             "value": "Updated Event Title"
        ///         },
        ///         {
        ///             "op": "replace",
        ///             "path": "/description",
        ///             "value": "This is an updated description for the event."
        ///         },
        ///         {
        ///             "op": "replace",
        ///             "path": "/endTime",
        ///             "value": "2023-10-15T12:00:00"
        ///         }
        ///     ]
        /// 
        /// Note:
        /// - `op` represents the operation. Mostly "replace" in our case.
        /// 
        /// - `path` is the field you want to update.
        /// 
        /// - `value` is the new value you want to set for the field.
        /// 
        /// </remarks>
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPatch("{eventId}/partial")]
        public async Task<ActionResult<EventResponse>> PartialUpdateEvent([FromRoute] int eventId, [FromBody] JsonPatchDocument<EventUpdateRequest> jsonPatch)
        {
            if (jsonPatch == null || jsonPatch.Operations.Count == 0)
                return BadRequest();

            var updateResult = await _EventService.PartialUpdateEventAsync(eventId, jsonPatch);

            if (updateResult.IsUnprocessable)
                return UnprocessableEntity(updateResult.ErrorMessage);

            return Ok(updateResult.Data);
        }

        /// <summary>
        /// Get list of event for a specific date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PartialEventResponse>))]
        [HttpGet("events/date/{date}", Name = "GetEventListForDate")]
        public async Task<ActionResult<IEnumerable<PartialEventResponse>>> GetEventListForDate([FromRoute] DateTime date)
        {
            var getResult = await _EventService.GetEventListForCurrentUserOnDate(date, UserId!);

            if (getResult.IsOk)
                return getResult.Data!.ToList();

            throw new ApplicationException(getResult.ErrorMessage);
        }

        /// <summary>
        /// Get list of event between dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PartialEventResponse>))]
        [HttpGet("events/range", Name = "GetEventListBetweenDates")]
        public async Task<ActionResult<IEnumerable<PartialEventResponse>>> GetEventListBetweenDates(
            [FromQuery, Required] DateTime startDate, [FromQuery, Required] DateTime endDate)
        {
            var getResult = await _EventService.GetEventListForCurrentUserBetweenDates(startDate, endDate, UserId!);

            if (getResult.IsOk)
                return getResult.Data!.ToList();
            else if (getResult.IsBadRequest)
                return BadRequest(getResult.ErrorMessage);

            throw new ApplicationException(getResult.ErrorMessage);
        }
    }
}
