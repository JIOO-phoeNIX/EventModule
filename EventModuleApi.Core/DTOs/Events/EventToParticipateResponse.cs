

namespace EventModuleApi.Core.DTOs.Events;

public class EventToParticipateResponse
{
    public string EventTitle { get; set; }
    public string EventDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TimeZone { get; set; }
    public string InvitationNote { get; set; }
    public string HostUsername { get; set; }
    public bool InviteAccepted { get; set; }
}