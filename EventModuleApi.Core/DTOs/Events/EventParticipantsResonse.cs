

namespace EventModuleApi.Core.DTOs.Events;

public class EventParticipantsResonse
{
    public string ParticipantName { get; set; }
    public string ParticipantEmail { get; set; }
    public bool IsApprovedByParticipant { get; set; }
    public string InvitationNote { get; set; }
}