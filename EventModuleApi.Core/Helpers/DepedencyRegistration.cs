using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Repository.Interfaces;
using EvenrModule.Persistence.Repository.Services;
using EventModuleApi.Core.Interfaces;
using EventModuleApi.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventModuleApi.Core.Helpers;

/// <summary>
/// This is used to handle dependency injection
/// </summary>
public static class DepedencyRegistration
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IEventRepository<Event>, EventRepository>();
        services.AddScoped<IEventParticipantRepository<EventParticipant>, EventParticipantRepository>();
    }
}