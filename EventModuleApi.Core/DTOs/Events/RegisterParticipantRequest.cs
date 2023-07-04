using System.ComponentModel.DataAnnotations;

namespace EventModuleApi.Core.DTOs.Events;

public class RegisterParticipantRequest
{
    [Required]
    public int EventId { get; set; }
    [Required]
    public int ParticipantUserId { get; set; }
    public string? InvitationNote { get; set; }
}
