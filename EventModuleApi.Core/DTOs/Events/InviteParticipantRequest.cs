using System.ComponentModel.DataAnnotations;

namespace EventModuleApi.Core.DTOs.Events;

public class InviteParticipantRequest
{
    /// <summary>
    /// This is the comma separated ids of the participants to invite
    /// </summary>
    [Required]
    public string ParticipantsId { get; set; }
    [Required]
    public int EventId { get; set; }
    public string InvitationNote { get; set; }
}