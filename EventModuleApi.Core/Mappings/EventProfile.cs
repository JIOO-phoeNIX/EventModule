using AutoMapper;
using EvenrModule.Persistence.Contexts;
using EventModuleApi.Core.DTOs.Events;

namespace EventModuleApi.Core.Mappings;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<CreateEventRequest, Event>();
        CreateMap<EventResponse, Event>().ReverseMap();
    }
}