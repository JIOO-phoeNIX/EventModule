using AutoMapper;
using EvenrModule.Persistence.Contexts;
using EventModuleApi.Core.DTOs.Events;

namespace EventModuleApi.Core.Mappings;

public class EventParticipantProfile : Profile
{
    public EventParticipantProfile()
    {
        CreateMap<EventParticipantsResonse, EventParticipant>();
    }
}
