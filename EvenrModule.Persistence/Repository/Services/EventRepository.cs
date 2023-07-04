using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvenrModule.Persistence.Repository.Services;

public class EventRepository : BaseRepository<Event>, IEventRepository<Event>
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> CountAllEventByUserId(int userId)
    {
        return await _context.Events.CountAsync(e => e.UserId == userId);
    }

    public async Task<IEnumerable<Event>> GetAllByUserIdAsync(int userId, int pageNumber, int pageSize)
    {
        return await _context.Events
            .Where(x => x.UserId == userId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}