using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Repository.Interfaces;
using EvenrModule.Persistence.Repository.Services;
using Microsoft.EntityFrameworkCore;

public class EventParticipantRepository : BaseRepository<EventParticipant>, IEventParticipantRepository<EventParticipant>
{
    private readonly ApplicationDbContext _context;

    public EventParticipantRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> CountAllEventByEventId(int eventId)
    {
        return await _context.EventParticipants.CountAsync(x => x.EventId == eventId);
    }

    public async Task<int> CountAllEventByUserId(int userId)
    {
        return await _context.EventParticipants.CountAsync(x => x.ParticipantUserId == userId);
    }

    public async Task<IEnumerable<EventParticipant>> GetAllByEventIdAsync(int eventId)
    {
        return await _context.EventParticipants.Where(x => x.EventId == eventId).ToListAsync();
    }

    public async Task<IEnumerable<EventParticipant>> GetAllByUserIdAsync(int userId, int pageNumber, int pageSize)
    {
        return await _context.EventParticipants
            .Where(x => x.ParticipantUserId == userId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<EventParticipant?> GetByEventAndParticipantIdAsync(int eventId, int participantId)
    {
        return await _context.EventParticipants
            .FirstOrDefaultAsync(x => x.EventId == eventId && x.ParticipantUserId == participantId);
    }

    public async Task<IEnumerable<EventParticipant>> GetPagedAllByEventIdAsync(int eventId, int pageNumber, int pageSize)
    {
        return await _context.EventParticipants
            .Where(x => x.EventId == eventId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}