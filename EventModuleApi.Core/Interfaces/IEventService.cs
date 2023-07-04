using EventModuleApi.Core.DTOs.Events;
using EventModuleApi.Core.Helpers;

namespace EventModuleApi.Core.Interfaces;

public interface IEventService
{
    public Task<ApiResponse> CreateEventAsync(CreateEventRequest createEventRequest);
    public Task<ApiResponse> EditEventAsync(EditEventRequest editEventRequest);
    public Task<ApiResponse> GetAllUserEvents(int userId, PaginationFilter paginationFilter);
    public Task<ApiResponse> EventInfo(int id);
    public Task<ApiResponse> ParticipantRegistration(RegisterParticipantRequest registerParticipantRequest);
    public Task<ApiResponse> GetAllParticipantsOfAnEvent(int eventId, PaginationFilter paginationFilter);
    public Task<ApiResponse> InviteParticipantsToEvent(InviteParticipantRequest request);
    public Task<ApiResponse> AcceptEventInvitation(RegisterParticipantRequest request);
    public Task<ApiResponse> AllEventsToParticipate(int userId, PaginationFilter paginationFilter);
}