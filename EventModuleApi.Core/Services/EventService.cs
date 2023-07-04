using AutoMapper;
using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Models.UserDetails;
using EvenrModule.Persistence.Repository.Interfaces;
using EventModuleApi.Core.DTOs.Events;
using EventModuleApi.Core.Helpers;
using EventModuleApi.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Threading;

namespace EventModuleApi.Core.Services;

public class EventService : IEventService
{
    private readonly IEventRepository<Event> _eventRepository;
    private readonly IEventParticipantRepository<EventParticipant> _eventParticipantRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<EventService> _logger;
    private readonly IMemoryCache _memoryCache;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public EventService(IEventRepository<Event> eventRepository, IUserService userService, IMapper mapper,
        IEventParticipantRepository<EventParticipant> eventParticipantRepository, ILogger<EventService> logger, IMemoryCache memoryCache)
    {
        _eventRepository = eventRepository;
        _userService = userService;
        _mapper = mapper;
        _eventParticipantRepository = eventParticipantRepository;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<ApiResponse> CreateEventAsync(CreateEventRequest createEventRequest)
    {
        try
        {
            //validate the userid
            var user = await _userService.GetUserDetailsAsync(createEventRequest.UserId);
            if (user is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid user id passed: {createEventRequest.UserId}", null);

            //validate the timezone
            bool checkTimeZone = IsTimeZoneValid(createEventRequest.TimeZoneDisplayName);
            if (checkTimeZone is false)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid time zone passed: {createEventRequest.TimeZoneDisplayName}", null);

            //store the event details in the data store
            var eventToStore = _mapper.Map<Event>(createEventRequest);

            eventToStore.DateCreated = DateTime.Now;
            eventToStore.LastDateUpdated = DateTime.Now;

            await _eventRepository.AddAsync(eventToStore);
            await _eventRepository.SaveChangesAsync();

            return new ApiResponse((int)HttpStatusCode.OK, "Event created successfully", null);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in CreateEventAsync message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> EditEventAsync(EditEventRequest editEventRequest)
    {
        try
        {
            //validate the timezone
            bool checkTimeZone = IsTimeZoneValid(editEventRequest.TimeZoneDisplayName);
            if (checkTimeZone is false)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid time zone passed: {editEventRequest.TimeZoneDisplayName}", null);

            //fetch the event
            var eventToEdit = await GetEventDetailsById(editEventRequest.EventId);
            if (eventToEdit is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {editEventRequest.EventId}", null);

            eventToEdit.Title = editEventRequest.Title;
            eventToEdit.StartDate = editEventRequest.StartDate;
            eventToEdit.EndDate = editEventRequest.EndDate;
            eventToEdit.Description = editEventRequest.Description;
            eventToEdit.TimeZoneDisplayName = editEventRequest.TimeZoneDisplayName;
            eventToEdit.LastDateUpdated = DateTime.Now;

            _eventRepository.Update(eventToEdit);
            await _eventRepository.SaveChangesAsync();

            //remove the item from the cache after updating 
            string key = "Event_" + editEventRequest.EventId;
            _memoryCache.Remove(key);

            return new ApiResponse((int)HttpStatusCode.OK, "Event updated successfully", null);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in EditEventAsync message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> GetAllUserEvents(int userId, PaginationFilter paginationFilter)
    {
        try
        {
            var allUserEvents = await _eventRepository.GetAllByUserIdAsync(userId, paginationFilter.PageNumber, paginationFilter.PageSize);

            var allRecordCount = await _eventRepository.CountAllEventByUserId(userId);
            var totalPages = Convert.ToInt32(Math.Ceiling((((double)allRecordCount / (double)paginationFilter.PageSize))));
            if (!allUserEvents.Any())
                return new PagedApiResponse((int)HttpStatusCode.BadRequest, "No event found for this user", null, paginationFilter.PageNumber, paginationFilter.PageSize,
                    totalPages, allRecordCount);

            var eventsToReturn = _mapper.Map<List<EventResponse>>(allUserEvents);

            return new PagedApiResponse((int)HttpStatusCode.OK, "All user events", eventsToReturn, paginationFilter.PageNumber, 
                paginationFilter.PageSize, totalPages, allRecordCount);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetAllUserEvents message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new PagedApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message, paginationFilter.PageNumber, paginationFilter.PageSize, 0, 0);
        }
    }

    public async Task<ApiResponse> EventInfo(int id)
    {
        try
        {
            //fetch the event
            var eventInfo = await GetEventDetailsById(id);
            if (eventInfo is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {id}", null);

            //get the user info
            var user = await _userService.GetUserDetailsAsync(eventInfo.UserId);
            if (user is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Error retrieving creator details: {eventInfo.UserId}", null);

            var eventInfoReturn = new EventInfoResponse
            {
                EventResponse = _mapper.Map<EventResponse>(eventInfo),
                User = user
            };

            return new ApiResponse((int)HttpStatusCode.OK, "All user events", eventInfoReturn);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in EventInfo message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> ParticipantRegistration(RegisterParticipantRequest registerParticipantRequest)
    {
        try
        {
            //fetch event
            var eventInfo = await GetEventDetailsById(registerParticipantRequest.EventId);
            if (eventInfo is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {registerParticipantRequest.EventId}", null);

            //event owner cannot register as participant
            if (eventInfo.UserId == registerParticipantRequest.ParticipantUserId)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You cannot register as participant for your event", null);

            //get the participant info
            var participant = await _userService.GetUserDetailsAsync(registerParticipantRequest.ParticipantUserId);
            if (participant is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid participant id: {registerParticipantRequest.ParticipantUserId}", null);

            //check if user already registered for event
            var participantInfo = await _eventParticipantRepository.GetByEventAndParticipantIdAsync(registerParticipantRequest.EventId, registerParticipantRequest.ParticipantUserId);

            //user registered or invited and accepted invite already
            if (participantInfo is not null && participantInfo.IsApprovedByParticipant)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You are already registered for this event", null);

            //user invited and yet to accept invite
            if (participantInfo is not null && participantInfo.IsApprovedByParticipant is false)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You have been invited to this event, please accept the invitation", null);

            //register and accept the invitation for the participant
            var eventParticipantToStore = new EventParticipant
            {
                DateCreated = DateTime.Now,
                LastDateUpdated = DateTime.Now,
                EventId = registerParticipantRequest.EventId,
                ParticipantUserId = registerParticipantRequest.ParticipantUserId,
                //invitation is accepted here since the participant is the one registering and event owner doesn't need to approve
                IsApprovedByParticipant = true,
                DateParticipantApproved = DateTime.Now,
                InvitationNote = registerParticipantRequest.InvitationNote
            };

            await _eventParticipantRepository.AddAsync(eventParticipantToStore);
            await _eventParticipantRepository.SaveChangesAsync();

            return new ApiResponse((int)HttpStatusCode.OK, "Participant registered successfully", null);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in ParticipantRegistration message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> GetAllParticipantsOfAnEvent(int eventId, PaginationFilter paginationFilter)
    {
        try
        {
            //fetch event
            var eventInfo = await GetEventDetailsById(eventId);
            if (eventInfo is null)
                return new PagedApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {eventId}", null,
                    paginationFilter.PageNumber, paginationFilter.PageSize, 0, 0);

            var allRecordCount = await _eventParticipantRepository.CountAllEventByEventId(eventId);
            var totalPages = Convert.ToInt32(Math.Ceiling((((double)allRecordCount / (double)paginationFilter.PageSize))));

            //fetch participants 
            var eventParticipants = await _eventParticipantRepository.GetPagedAllByEventIdAsync(eventId, paginationFilter.PageNumber, paginationFilter.PageSize);
            if (eventParticipants is null || eventParticipants.Count() == 0)
                return new PagedApiResponse((int)HttpStatusCode.BadRequest, $"No user has registered for this event", null,
                    paginationFilter.PageNumber, paginationFilter.PageSize, totalPages, allRecordCount);

            var eventParticipantsToReturn = new List<EventParticipantsResonse>();

            foreach (var participant in eventParticipants)
            {
                var tempUser = await _userService.GetUserDetailsAsync(participant.ParticipantUserId);
                if (tempUser is null)
                    continue;

                eventParticipantsToReturn.Add(new EventParticipantsResonse
                {
                    IsApprovedByParticipant = participant.IsApprovedByParticipant,
                    ParticipantEmail = tempUser.email,
                    ParticipantName = tempUser.name,
                    InvitationNote = participant.InvitationNote
                });
            }

            return new PagedApiResponse((int)HttpStatusCode.OK, "Participants details", eventParticipantsToReturn,
                paginationFilter.PageNumber, paginationFilter.PageSize, totalPages, allRecordCount);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetAllPArticipantsOfAnEvent message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new PagedApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message,
                paginationFilter.PageNumber, paginationFilter.PageSize, 0, 0);
        }
    }

    public async Task<ApiResponse> InviteParticipantsToEvent(InviteParticipantRequest request)
    {
        try
        {
            //fetch event
            var eventInfo = await GetEventDetailsById(request.EventId);
            if (eventInfo is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {request.EventId}", null);

            //fetch participants 
            var eventParticipants = await _eventParticipantRepository.GetAllByEventIdAsync(request.EventId);

            //get the users id
            var userIds = request.ParticipantsId.Split(',');

            int countOfSuccessfulInvitations = 0;

            //validate the users exist and add as participant
            foreach (var userIdString in userIds)
            {
                //check if id is valid
                bool isParsable = Int32.TryParse(userIdString, out int userId);
                if (isParsable is false)
                    continue;

                //check if user already registered for this event
                if (eventParticipants.Any(x => x.ParticipantUserId == userId))
                    continue;

                //check if user id is valid
                var tempUser = await _userService.GetUserDetailsAsync(userId);
                if (tempUser is null)
                    continue;

                //register the participant
                var eventParticipantToStore = new EventParticipant
                {
                    DateCreated = DateTime.Now,
                    LastDateUpdated = DateTime.Now,
                    EventId = request.EventId,
                    ParticipantUserId = userId,
                    //participant must approve invite from owner
                    IsApprovedByParticipant = false,
                    InvitationNote = request.InvitationNote
                };

                countOfSuccessfulInvitations++;
                await _eventParticipantRepository.AddAsync(eventParticipantToStore);
            }

            await _eventParticipantRepository.SaveChangesAsync();

            return new ApiResponse((int)HttpStatusCode.OK, $"{countOfSuccessfulInvitations} participant(s) invited successfully", null);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in InviteParticipantsToEvent message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> AcceptEventInvitation(RegisterParticipantRequest request)
    {
        try
        {
            //fetch event
            var eventInfo = await GetEventDetailsById(request.EventId);
            if (eventInfo is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid event is passed: {request.EventId}", null);

            //event owner cannot register as participant
            if (eventInfo.UserId == request.ParticipantUserId)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You cannot register as participant for your event", null);

            //get the participant info
            var participant = await _userService.GetUserDetailsAsync(request.ParticipantUserId);
            if (participant is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"Invalid participant id: {request.ParticipantUserId}", null);

            //check if user already registered for event
            var participantInfo = await _eventParticipantRepository.GetByEventAndParticipantIdAsync(request.EventId, request.ParticipantUserId);

            //user registered or invited and accepted invite already
            if (participantInfo is null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You are not invited to this event", null);

            //user registered or invited and accepted invite already
            if (participantInfo is not null && participantInfo.IsApprovedByParticipant)
                return new ApiResponse((int)HttpStatusCode.BadRequest, $"You are already registered for this event", null);

            participantInfo.IsApprovedByParticipant = true;
            participantInfo.DateParticipantApproved = DateTime.Now;

            _eventParticipantRepository.Update(participantInfo);
            await _eventParticipantRepository.SaveChangesAsync();

            return new ApiResponse((int)HttpStatusCode.OK, $"Invitation accepted successfully", null);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AcceptEventInvitation message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new ApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message);
        }
    }

    public async Task<ApiResponse> AllEventsToParticipate(int userId, PaginationFilter paginationFilter)
    {
        try
        {
            //get the user info
            var userDetails = await _userService.GetUserDetailsAsync(userId);
            if (userDetails is null)
                return new PagedApiResponse((int)HttpStatusCode.BadRequest, $"Invalid user id: {userId}", null,
                    paginationFilter.PageNumber, paginationFilter.PageSize, 0, 0);

            //get all user event to participate
            var allEventToParticipate = await _eventParticipantRepository.GetAllByUserIdAsync(userId, paginationFilter.PageNumber, paginationFilter.PageSize);

            var allRecordCount = await _eventParticipantRepository.CountAllEventByUserId(userId);
            var totalPages = Convert.ToInt32(Math.Ceiling((((double)allRecordCount / (double)paginationFilter.PageSize))));

            if (allEventToParticipate.Count() == 0)
                return new PagedApiResponse((int)HttpStatusCode.BadRequest, "You are not invited to any event", null,
                    paginationFilter.PageNumber, paginationFilter.PageSize, totalPages, allRecordCount);

            var eventsToParticipate = new List<EventToParticipateResponse>();

            foreach (var eventToParticipant in allEventToParticipate)
            {
                var tempEventInfo = await GetEventDetailsById(eventToParticipant.EventId);
                if (tempEventInfo is null)
                    continue;

                var hostDetails = await _userService.GetUserDetailsAsync(tempEventInfo.UserId);
                if (hostDetails is null)
                    continue;

                eventsToParticipate.Add(new EventToParticipateResponse
                {
                    EndDate = tempEventInfo.EndDate,
                    EventDescription = tempEventInfo.Description,
                    EventTitle = tempEventInfo.Title,
                    HostUsername = hostDetails.username,
                    InvitationNote = eventToParticipant?.InvitationNote,
                    StartDate = tempEventInfo.StartDate,
                    TimeZone = tempEventInfo.TimeZoneDisplayName,
                    InviteAccepted = eventToParticipant.IsApprovedByParticipant
                });
            }

            return new PagedApiResponse((int)HttpStatusCode.OK, $"All events to participate", eventsToParticipate,
                paginationFilter.PageNumber, paginationFilter.PageSize, totalPages, allRecordCount);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AllEventsToParticipate message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return new PagedApiResponse((int)HttpStatusCode.InternalServerError, "An error occurred: ", ex.Message,
                paginationFilter.PageNumber, paginationFilter.PageSize, 0, 0);
        }
    }

    #region Helper functions

    /// <summary>
    /// Validate the value passed in the timezone
    /// </summary>
    /// <param name="timezone">sample value to use "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"</param>
    /// <returns>True or false if the timezone string is a valid timezone or not</returns>
    public bool IsTimeZoneValid(string timezone)
    {
        var timeZonesName = TimeZoneInfo
                .GetSystemTimeZones()
                .ToDictionary(x => x.DisplayName);

        return timeZonesName.TryGetValue(timezone, out TimeZoneInfo? tz);
    }

    /// <summary>
    /// Fetch an event from the data store or cache
    /// </summary>
    /// <param name="eventId">The Id of the event</param>
    /// <returns>Null or the event</returns>
    public async Task<Event?> GetEventDetailsById(int eventId)
    {
        string key = "Event_" + eventId;
        if (!_memoryCache.TryGetValue(key, out Event eventToReturn))
        {
            await _semaphore.WaitAsync();
            if (!_memoryCache.TryGetValue(key, out eventToReturn))
            {
                var eventInfo = await _eventRepository.GetByIdAsync(eventId);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(20)
                };
                _memoryCache.Set(key, eventInfo, cacheEntryOptions);
                _memoryCache.TryGetValue(key, out eventToReturn);
                _semaphore.Release();
            }
        }

        return eventToReturn;
    }

    #endregion
}