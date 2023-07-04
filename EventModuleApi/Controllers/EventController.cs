using EventModuleApi.Core.DTOs.Events;
using EventModuleApi.Core.Helpers;
using EventModuleApi.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EventModuleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    public readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost("add")]
    public async Task<object> CreateEvent(CreateEventRequest request)
    {
        var result = await _eventService.CreateEventAsync(request);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update")]
    public async Task<object> EditEvent(EditEventRequest request)
    {
        var result = await _eventService.EditEventAsync(request);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpGet("alluserevents/{userId}")]
    public async Task<object> GetUserEvents([FromRoute][Required] int userId, [FromQuery][Required]int pageNumber, [FromQuery][Required] int pageSize)
    {
        var paginationFilter = new PaginationFilter(pageNumber, pageSize);
        var result = await _eventService.GetAllUserEvents(userId, paginationFilter);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpGet("info/{id}")]
    public async Task<object> GetEventInfo([FromRoute][Required] int id)
    {
        var result = await _eventService.EventInfo(id);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpPost("participantregistration")]
    public async Task<object> RegisterParticipant(RegisterParticipantRequest request)
    {
        var result = await _eventService.ParticipantRegistration(request);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpGet("alleventparticipants/{eventId}")]
    public async Task<object> GetAllParticipantsOfAnEvent([FromRoute][Required] int eventId, [FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize)
    {
        var paginationFilter = new PaginationFilter(pageNumber, pageSize);
        var result = await _eventService.GetAllParticipantsOfAnEvent(eventId, paginationFilter);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpPost("inviteparticipants")]
    public async Task<object> InviteParticipants(InviteParticipantRequest request)
    {
        var result = await _eventService.InviteParticipantsToEvent(request);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpPost("acceptinivitationtoevent")]
    public async Task<object> AcceptEventInvitation(RegisterParticipantRequest request)
    {
        var result = await _eventService.AcceptEventInvitation(request);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }

    [HttpGet("alleventstoparticipate/{userId}")]
    public async Task<object> AllEventsToParticipate([FromRoute][Required] int userId, [FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize)
    {
        var paginationFilter = new PaginationFilter(pageNumber, pageSize);
        var result = await _eventService.AllEventsToParticipate(userId, paginationFilter);

        return result.Code == 200 ? Ok(result) : BadRequest(result);
    }
}