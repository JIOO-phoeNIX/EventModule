using System.ComponentModel.DataAnnotations;

namespace EventModuleApi.Core.DTOs.Events;

public class CreateEventRequest
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string TimeZoneDisplayName { get; set; }
}