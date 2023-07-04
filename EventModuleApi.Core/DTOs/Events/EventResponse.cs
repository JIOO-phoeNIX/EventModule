

namespace EventModuleApi.Core.DTOs.Events;

public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }
    public string TimeZoneDisplayName { get; set; }
}