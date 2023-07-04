

namespace EvenrModule.Persistence.Repository.Interfaces;

public interface IEventRepository<T> : IBaseRepository<T> where T : class
{
    //Add other entity based db operations 
    public Task<IEnumerable<T>> GetAllByUserIdAsync(int userId, int pageNumber, int pageSize);
    public Task<int> CountAllEventByUserId(int userId);
}