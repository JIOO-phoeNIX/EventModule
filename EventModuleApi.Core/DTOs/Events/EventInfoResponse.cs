using EvenrModule.Persistence.Models.UserDetails;

namespace EventModuleApi.Core.DTOs.Events;

public class EventInfoResponse
{
    public EventResponse EventResponse { get; set; } = new EventResponse();
    public User User { get; set; } = new User();
}