using AutoMapper;
using Calendar.DataAccess.Infra;
using Calendar.DataAccess.Interfaces;
using Calendar.Services.Services;
using Calendar.shared.Entities;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Response;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;

namespace Calendar.Service.Tests.Services
{
    [TestClass]
    public class EventServiceTests
    {
        private Mock<IEventRepository>      _mockEventRepository;
        private Mock<ILogger<EventService>> _mockLogger;
        private Mock<IMapper>               _mockMapper;
        private EventService                _eventService;

        private const int EVENT_ID = 1;

        [TestInitialize]
        public void Setup()
        {
            // Initialize Mocks
            _mockEventRepository = new Mock<IEventRepository>();
            _mockLogger = new Mock<ILogger<EventService>>();
            _mockMapper = new Mock<IMapper>();

            // Create instance of the service under test
            _eventService = new EventService(_mockEventRepository.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetEventbyIdAsync_ValidId_ReturnsEventResponse()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };
            var eventResponse = new EventResponse(EVENT_ID,
                entity.Title,
                entity.Description,
                entity.AllDay,
                entity.StartTime,
                entity.EndTime);

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(entity));

            _mockMapper.Setup(m => m.Map<EventResponse>(entity))
                .Returns(eventResponse);

            // Act
            var result = await _eventService.GetEventbyIdAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(eventResponse.Id, EVENT_ID);
            Assert.AreEqual(eventResponse.Title, entity.Title);
            Assert.AreEqual(eventResponse.Description, entity.Description);
            Assert.AreEqual(eventResponse.AllDay, entity.AllDay);
            Assert.AreEqual(eventResponse.StartTime, entity.StartTime);
            Assert.AreEqual(eventResponse.EndTime, entity.EndTime);
        }

        [TestMethod]
        public async Task GetEventbyIdAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.NotFound(EVENT_ID));

            // Act
            var result = await _eventService.GetEventbyIdAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task GetEventbyIdAsync_ErrorOccurs_ReturnsError()
        {
            // Arrange
            var exceptionMessage = $"An error happened trying to get Event for event id : [{EVENT_ID}]";

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Error(new Exception(exceptionMessage), exceptionMessage));

            // Act
            var result = await _eventService.GetEventbyIdAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(exceptionMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task AddEventAsync_ValidRequest_ReturnsEventResponse()
        {
            // Arrange
            var createRequest = new EventCreateRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };

            var userId = "TestUserId";
            var newEventEntity = new Event
            {
                Title = createRequest.Title,
                Description = createRequest.Description,
                StartTime = createRequest.StartTime,
                EndTime = createRequest.EndTime,
                UserId = userId,
            };

            _mockEventRepository.Setup(er => er.AddAsync(newEventEntity))
                .ReturnsAsync(RepositoryActionResult.Ok());

            _mockMapper.Setup(m => m.Map<Event>(createRequest))
                .Returns(newEventEntity);

            // Act
            var result = await _eventService.AddEventAsync(createRequest, userId);

            // Assert
            Assert.IsTrue(result.IsOk);
        }

        [TestMethod]
        public async Task AddEventAsync_ValidRequestAllDay_ReturnsEventResponse()
        {
            // Arrange
            var createRequest = new EventCreateRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                AllDay = true,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
            };

            var userId = "TestUserId";
            var newEventEntity = new Event
            {
                Title = createRequest.Title,
                Description = createRequest.Description,
                StartTime = createRequest.StartTime.Date,
                EndTime = createRequest.EndTime.Date,
                UserId = userId,
            };

            _mockEventRepository.Setup(er => er.AddAsync(newEventEntity))
                .ReturnsAsync(RepositoryActionResult.Ok());

            _mockMapper.Setup(m => m.Map<Event>(createRequest))
                .Returns(newEventEntity);

            // Act
            var result = await _eventService.AddEventAsync(createRequest, userId);

            // Assert
            Assert.IsTrue(result.IsOk);
        }

        [TestMethod]
        public async Task AddEventAsync_InvalidDates_ReturnsUnprocessable()
        {
            // Arrange
            var createRequest = new EventCreateRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                AllDay = false,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now,
            };

            var userId = "TestUserId";

            // Act
            var result = await _eventService.AddEventAsync(createRequest, userId);

            // Assert
            Assert.IsTrue(result.IsUnprocessable);
            Assert.AreEqual("Start time should be before end time.", result.ErrorMessage);
        }

        [TestMethod]
        public async Task AddEventAsync_AllDayInvalidDates_ReturnsUnprocessable()
        {
            // Arrange
            var createRequest = new EventCreateRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                AllDay = true,
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddHours(10),
            };

            var userId = "TestUserId";

            // Act
            var result = await _eventService.AddEventAsync(createRequest, userId);

            // Assert
            Assert.IsTrue(result.IsUnprocessable);
            Assert.AreEqual("Start time should be before end time.", result.ErrorMessage);
        }

        [TestMethod]
        public async Task AddEventAsync_ErrorDuringSave_ReturnsError()
        {
            // Arrange
            var createRequest = new EventCreateRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };

            var userId = "TestUserId";
            var newEventEntity = new Event
            {
                Title = createRequest.Title,
                Description = createRequest.Description,
                StartTime = createRequest.StartTime,
                EndTime = createRequest.EndTime,
                UserId = userId,
            };
            newEventEntity.UserId = userId;

            var exceptionMessage = "Save to database Failed.";

            _mockEventRepository.Setup(er => er.AddAsync(newEventEntity))
                .ReturnsAsync(RepositoryActionResult.Error(new Exception(exceptionMessage), exceptionMessage));

            _mockMapper.Setup(m => m.Map<Event>(createRequest))
                .Returns(newEventEntity);

            // Act
            var result = await _eventService.AddEventAsync(createRequest, userId);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(exceptionMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task DeleteEventAsync_ValidEventId_ReturnsOk()
        {
            // Arrange
            _mockEventRepository.Setup(er => er.DeleteAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult.Ok());

            // Act
            var result = await _eventService.DeleteEventAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsOk);
        }

        [TestMethod]
        public async Task DeleteEventAsync_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockEventRepository.Setup(er => er.DeleteAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult.NotFound(EVENT_ID));

            // Act
            var result = await _eventService.DeleteEventAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task DeleteEventAsync_ErrorDuringDeletion_ReturnsError()
        {
            // Arrange
            var exceptionMessage = $"Failed to delete event for Event id: [{EVENT_ID}]";

            _mockEventRepository.Setup(er => er.DeleteAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult.Error(new Exception(exceptionMessage), exceptionMessage));

            // Act
            var result = await _eventService.DeleteEventAsync(EVENT_ID);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(exceptionMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task PartialUpdateEventAsync_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<EventUpdateRequest>();
            patchDocument.Replace(e => e.Title, "NewTitle");

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.NotFound(EVENT_ID));

            // Act
            var result = await _eventService.PartialUpdateEventAsync(EVENT_ID, patchDocument);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task PartialUpdateEventAsync_TitleAndDescriptionValidPatch_ReturnsUpdatedEvent()
        {
            // Arrange
            var originalEvent = new Event { 
                Id = EVENT_ID, 
                Title = "OriginalTitle",
                Description = "OriginalDescription",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var patchDocument = new JsonPatchDocument<EventUpdateRequest>();
            patchDocument.Replace(e => e.Title, "NewTitle");
            patchDocument.Replace(e => e.Description, "NewDescription");

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEvent));

            _mockMapper.SetupSequence(m => m.Map<EventUpdateRequest>(originalEvent))
                .Returns(new EventUpdateRequest { Title = "OriginalTitle", Description = "OriginalDescription" })
                .Returns(new EventUpdateRequest { Title = "NewTitle", Description = "NewDescription" });

            _mockEventRepository.Setup(er => er.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(RepositoryActionResult.Ok());

            var updatedEventResponse = new EventResponse(EVENT_ID,
                "NewTitle",
                "NewDescription",
                originalEvent.AllDay,
                originalEvent.StartTime,
                originalEvent.EndTime);
            _mockMapper.Setup(m => m.Map<EventResponse>(It.IsAny<Event>()))
                .Returns(updatedEventResponse);

            // Act
            var result = await _eventService.PartialUpdateEventAsync(EVENT_ID, patchDocument);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual("NewTitle", result.Data!.Title);
            Assert.AreEqual("NewDescription", result.Data!.Description);
        }

        [TestMethod]
        public async Task PartialUpdateEventAsync_StartAndEndTimeValidPatch_ReturnsUpdatedEvent()
        {
            // Arrange
            var originalEvent = new Event {
                Id = EVENT_ID,
                Title = "Title",
                Description = "Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var newStartTime = DateTime.Now.AddHours(2);
            var newEndTime = DateTime.Now.AddHours(3);

            var patchDocument = new JsonPatchDocument<EventUpdateRequest>();
            patchDocument.Replace(e => e.StartTime, newStartTime);
            patchDocument.Replace(e => e.EndTime, newEndTime);

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEvent));

            _mockMapper.SetupSequence(m => m.Map<EventUpdateRequest>(originalEvent))
                .Returns(new EventUpdateRequest { StartTime = originalEvent.StartTime, EndTime = originalEvent.EndTime });

            _mockMapper.Setup(m => m.Map(It.IsAny<EventUpdateRequest>(), It.IsAny<Event>()))
               .Callback<EventUpdateRequest, Event>((src, dest) =>
               {
                   dest.StartTime = src.StartTime;
                   dest.EndTime = src.EndTime;
               });

            _mockEventRepository.Setup(er => er.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(RepositoryActionResult.Ok());

            var updatedEventResponse = new EventResponse(EVENT_ID,
                originalEvent.Title,
                originalEvent.Description,
                originalEvent.AllDay,
                newStartTime,
                newEndTime);
            _mockMapper.Setup(m => m.Map<EventResponse>(It.IsAny<Event>()))
                .Returns(updatedEventResponse);

            // Act
            var result = await _eventService.PartialUpdateEventAsync(EVENT_ID, patchDocument);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(newStartTime, result.Data!.StartTime);
            Assert.AreEqual(newEndTime, result.Data!.EndTime);
        }

        [TestMethod]
        public async Task PartialUpdateEventAsync_StartTimeAfterEndTime_Unprocessable()
        {
            // Arrange
            var originalEvent = new Event {
                Id = EVENT_ID, 
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var patchDocument = new JsonPatchDocument<EventUpdateRequest>();
            patchDocument.Replace(e => e.StartTime, DateTime.Now.AddHours(3));
            patchDocument.Replace(e => e.EndTime, DateTime.Now.AddHours(2));

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEvent));

            _mockMapper.SetupSequence(m => m.Map<EventUpdateRequest>(originalEvent))
                .Returns(new EventUpdateRequest { StartTime =originalEvent.StartTime, EndTime = originalEvent.EndTime });

            _mockMapper.Setup(m => m.Map(It.IsAny<EventUpdateRequest>(), It.IsAny<Event>()))
               .Callback<EventUpdateRequest, Event>((src, dest) =>
               {
                   dest.StartTime = src.StartTime;
                   dest.EndTime = src.EndTime;
               });

            // Act
            var result = await _eventService.PartialUpdateEventAsync(EVENT_ID, patchDocument);

            // Assert
            Assert.IsTrue(result.IsUnprocessable);
        }

        [TestMethod]
        public async Task PartialUpdateEventAsync_EventFetchError_ReturnsError()
        {
            // Arrange
            var exceptionMessage = $"Failed to Update Event for event id : [{EVENT_ID}]";
            var patchDocument = new JsonPatchDocument<EventUpdateRequest>();
            patchDocument.Replace(e => e.Title, "NewTitle");

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Error(new Exception(exceptionMessage), exceptionMessage));

            // Act
            var result = await _eventService.PartialUpdateEventAsync(EVENT_ID, patchDocument);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(exceptionMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateEventAsync_ValidRequest_UpdatesEventSuccessfully()
        {
            // Arrange
            var updateRequest = new EventUpdateRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var originalEventEntity = new Event
            {
                Id = EVENT_ID,
                Title = "Original Title",
                Description = "Original Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var updatedEventEntity = new Event
            {
                Id = EVENT_ID,
                Title = updateRequest.Title,
                Description = updateRequest.Description,
                AllDay = false,
                StartTime = updateRequest.StartTime,
                EndTime = updateRequest.EndTime
            };
            var updatedEventResponse = new EventResponse(updatedEventEntity.Id,
                updatedEventEntity.Title,
                updatedEventEntity.Description,
                updatedEventEntity.AllDay,
                updatedEventEntity.StartTime,
                updatedEventEntity.EndTime);

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEventEntity));

            _mockEventRepository.Setup(er => er.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(RepositoryActionResult.Ok());

            _mockMapper.Setup(m => m.Map<EventResponse>(It.IsAny<Event>()))
                .Returns(updatedEventResponse);

            // Act
            var result = await _eventService.UpdateEventAsync(EVENT_ID, updateRequest);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(updatedEventResponse.Title, result.Data!.Title);
            Assert.AreEqual(updatedEventResponse.Description, result.Data!.Description);
            Assert.AreEqual(updatedEventResponse.StartTime, result.Data!.StartTime);
            Assert.AreEqual(updatedEventResponse.EndTime, result.Data!.EndTime);
        }

        [TestMethod]
        public async Task UpdateEventAsync_InvalidTimeRange_ReturnsUnprocessable()
        {
            // Arrange
            var originalEventEntity = new Event
            {
                Id = EVENT_ID,
                Title = "Original Title",
                Description = "Original Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var updateRequest = new EventUpdateRequest
            {
                Title = "Title",
                Description = "Description",
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now
            };

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEventEntity));

            // Act
            var result = await _eventService.UpdateEventAsync(EVENT_ID, updateRequest);

            // Assert
            Assert.IsTrue(result.IsUnprocessable);
        }

        [TestMethod]
        public async Task UpdateEventAsync_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateRequest = new EventUpdateRequest();

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.NotFound(EVENT_ID));

            // Act
            var result = await _eventService.UpdateEventAsync(EVENT_ID, updateRequest);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task UpdateEventAsync_ErrorFetchingEvent_ReturnsError()
        {
            // Arrange
            var updateRequest = new EventUpdateRequest();
            var errorMessage = $"Failed to Update Event for event id : [{EVENT_ID}].";

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Error(new Exception(errorMessage), errorMessage));

            // Act
            var result = await _eventService.UpdateEventAsync(EVENT_ID, updateRequest);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateEventAsync_ErrorUpdatingEvent_ReturnsError()
        {
            // Arrange
            var originalEventEntity = new Event
            {
                Id = EVENT_ID,
                Title = "Original Title",
                Description = "Original Description",
                AllDay = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var updateRequest = new EventUpdateRequest
            {
                Title = "Title",
                Description = "Description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var errorMessage = $"Failed to Update Event for event id : [{EVENT_ID}].";

            _mockEventRepository.Setup(er => er.GetByIdAsync(EVENT_ID))
                .ReturnsAsync(RepositoryActionResult<Event>.Ok(originalEventEntity));

            _mockEventRepository.Setup(er => er.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(RepositoryActionResult.Error(new Exception(errorMessage), errorMessage));

            // Act
            var result = await _eventService.UpdateEventAsync(EVENT_ID, updateRequest);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetEventListForCurrentUserOnDate_ValidRequest_ReturnsEvents()
        {
            // Arrange
            var dateToCheck = DateTime.Now.Date;
            var userId = Guid.NewGuid().ToString();
            var eventsFromDb = new List<Event>
            {
                new Event
                {
                    Id = EVENT_ID,
                    Title = "Title1",
                    Description = "Description1",
                    AllDay = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                },
                new Event
                {
                    Id = EVENT_ID + 1,
                    Title = "Title2",
                    Description = "Description2",
                    AllDay = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                },
            };
            var mappedEvents = new List<PartialEventResponse>
            {
                new PartialEventResponse(eventsFromDb[0].Id,
                    eventsFromDb[0].Title,
                    eventsFromDb[0].AllDay,
                    eventsFromDb[0].StartTime,
                    eventsFromDb[0].EndTime),
                new PartialEventResponse(eventsFromDb[1].Id,
                    eventsFromDb[1].Title,
                    eventsFromDb[1].AllDay,
                    eventsFromDb[1].StartTime,
                    eventsFromDb[1].EndTime)
            };

            _mockEventRepository.Setup(er => er.GetEventsForUserOnDateAsync(userId, dateToCheck))
                .ReturnsAsync(RepositoryActionResult<IReadOnlyList<Event>>.Ok(eventsFromDb));

            _mockMapper.Setup(m => m.Map<IReadOnlyList<PartialEventResponse>>(eventsFromDb))
                .Returns(mappedEvents);

            // Act
            var result = await _eventService.GetEventListForCurrentUserOnDate(dateToCheck, userId);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(mappedEvents.Count, result.Data!.Count);
        }

        [TestMethod]
        public async Task GetEventListForCurrentUserOnDate_ErrorFetchingEvents_ReturnsError()
        {
            // Arrange
            var dateToCheck = DateTime.Now.Date;
            var userId = Guid.NewGuid().ToString();
            var errorMessage = $"GetEventsForUserAndDateAsync failed for [{userId}] and date [{dateToCheck}]";

            _mockEventRepository.Setup(er => er.GetEventsForUserOnDateAsync(userId, dateToCheck))
                .ReturnsAsync(RepositoryActionResult<IReadOnlyList<Event>>.Error(new Exception(errorMessage), errorMessage));

            // Act
            var result = await _eventService.GetEventListForCurrentUserOnDate(dateToCheck, userId);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetEventListForCurrentUserBetweenDates_ValidRequest_ReturnsEvents()
        {
            // Arrange
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(2);
            var userId = Guid.NewGuid().ToString();
            var eventsFromDb = new List<Event>
            {
                new Event
                {
                    Id = EVENT_ID,
                    Title = "Title1",
                    Description = "Description1",
                    AllDay = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                },
                new Event
                {
                    Id = EVENT_ID + 1,
                    Title = "Title2",
                    Description = "Description2",
                    AllDay = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                }
            };
            var expectedEvents = new List<PartialEventResponse>
            {
                new PartialEventResponse(eventsFromDb[0].Id,
                    eventsFromDb[0].Title,
                    eventsFromDb[0].AllDay,
                    eventsFromDb[0].StartTime,
                    eventsFromDb[0].EndTime),
                new PartialEventResponse(eventsFromDb[1].Id,
                    eventsFromDb[1].Title,
                    eventsFromDb[1].AllDay,
                    eventsFromDb[1].StartTime,
                    eventsFromDb[1].EndTime)
            };

            _mockEventRepository.Setup(er => er.GetEventsForUserBetweenDatesAsync(userId, startDate, endDate))
                .ReturnsAsync(RepositoryActionResult<IReadOnlyList<Event>>.Ok(eventsFromDb));

            _mockMapper.Setup(m => m.Map<IReadOnlyList<PartialEventResponse>>(eventsFromDb))
                .Returns(expectedEvents);

            // Act
            var result = await _eventService.GetEventListForCurrentUserBetweenDates(startDate, endDate, userId);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(expectedEvents.Count, result.Data!.Count);
        }

        [TestMethod]
        public async Task GetEventListForCurrentUserBetweenDates_InvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var startDate = DateTime.Now.Date.AddDays(2);
            var endDate = DateTime.Now.Date;
            var userId = Guid.NewGuid().ToString();

            // Act
            var result = await _eventService.GetEventListForCurrentUserBetweenDates(startDate, endDate, userId);

            // Assert
            Assert.IsTrue(result.IsBadRequest);
            Assert.AreEqual("start date must be inferior or equal to end date.", result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetEventListForCurrentUserBetweenDates_ErrorFetchingEvents_ReturnsError()
        {
            // Arrange
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(2);
            var userId = Guid.NewGuid().ToString();
            var errorMessage = $"GetEventsForUserBetweenDateAsync failed for [{userId}], start date [{startDate}] and end date [{endDate}]";

            _mockEventRepository.Setup(er => er.GetEventsForUserBetweenDatesAsync(userId, startDate, endDate))
                .ReturnsAsync(RepositoryActionResult<IReadOnlyList<Event>>.Error(new Exception(errorMessage), errorMessage));

            // Act
            var result = await _eventService.GetEventListForCurrentUserBetweenDates(startDate, endDate, userId);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }
    }
}
