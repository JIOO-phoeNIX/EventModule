using System.ComponentModel.DataAnnotations;

namespace EvenrModule.Persistence.Contexts;

public class EventParticipant
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int EventId { get; set; }
    public string? InvitationNote { get; set; }
    [Required] 
    public int ParticipantUserId { get; set; }
    public bool IsApprovedByParticipant { get; set; }
    [Required]
    public DateTime DateCreated { get; set; }
    [Required]
    public DateTime LastDateUpdated { get; set; }
    public DateTime DateParticipantApproved { get; set; }
}