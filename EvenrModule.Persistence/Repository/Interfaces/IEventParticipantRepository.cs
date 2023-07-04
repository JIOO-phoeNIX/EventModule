using EvenrModule.Persistence.Contexts;

namespace EvenrModule.Persistence.Repository.Interfaces;

public interface IEventParticipantRepository<T> : IBaseRepository<T> where T : class
{
    public Task<EventParticipant?> GetByEventAndParticipantIdAsync(int eventId, int participantId);
    public Task <IEnumerable<T>> GetAllByEventIdAsync(int eventId);
    public Task<int> CountAllEventByEventId(int eventId);
    public Task <IEnumerable<T>> GetAllByUserIdAsync(int userId, int pageNumber, int pageSize);
    public Task<int> CountAllEventByUserId(int userId);
    public Task<IEnumerable<T>> GetPagedAllByEventIdAsync(int eventId, int pageNumber, int pageSize);
}