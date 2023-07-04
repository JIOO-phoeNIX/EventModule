using System.ComponentModel.DataAnnotations;

namespace EvenrModule.Persistence.Contexts;

public class Event
{
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// This is the ID of the user creating the event
    /// </summary>
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
    public DateTime DateCreated { get; set; }
    [Required]
    public DateTime LastDateUpdated { get; set; }
    [Required]
    public string TimeZoneDisplayName { get; set; }
}