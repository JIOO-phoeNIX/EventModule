using EvenrModule.Persistence.Models.UserDetails;

namespace EventModuleApi.Core.Interfaces;

public interface IUserService
{
    public Task<User?> GetUserDetailsAsync(int userId);
}