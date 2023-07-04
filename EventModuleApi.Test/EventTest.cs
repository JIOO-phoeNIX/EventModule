using AutoMapper;
using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Models.UserDetails;
using EvenrModule.Persistence.Repository.Interfaces;
using EvenrModule.Persistence.Repository.Services;
using EventModuleApi.Core.Interfaces;
using EventModuleApi.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EventModuleApi.Test;

public class EventTest
{
    private readonly Mock<IEventRepository<Event>> _eventRepository;
    private readonly Mock<IEventParticipantRepository<EventParticipant>> _eventParticipantRepository;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILogger<EventService>> _logger;
    private readonly Mock<IMemoryCache> _memoryCache;

    public EventTest()
    {
        _eventRepository = new Mock<IEventRepository<Event>>();
        _eventParticipantRepository = new Mock<IEventParticipantRepository<EventParticipant>>();
        _userService = new Mock<IUserService>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<EventService>>();
        _memoryCache = new Mock<IMemoryCache>();
    }

    [Theory]
    [InlineData("(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna")]
    [InlineData("(UTC+01:00) West Central Africa")]
    [InlineData("(UTC-08:00) Pacific Time (US & Canada)")]
    public void TimeZone_IsValid(string timeZoneString)
    {
        //Arrange
        var eventService = new EventService(_eventRepository.Object, _userService.Object, _mapper.Object,
           _eventParticipantRepository.Object, _logger.Object, _memoryCache.Object);

        //Act
        var timeZOneIsCorrect = eventService.IsTimeZoneValid(timeZoneString);

        //Assert
        Assert.True(timeZOneIsCorrect);
    }

    [Theory]
    [InlineData("(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm")]
    [InlineData("(UTC+01:00)")]
    [InlineData("Pacific Time (US & Canada)")]
    public void TimeZone_IsNotValid(string timeZoneString)
    {
        //Arrange
        var eventService = new EventService(_eventRepository.Object, _userService.Object, _mapper.Object,
           _eventParticipantRepository.Object, _logger.Object, _memoryCache.Object);

        //Act
        var timeZOneIsCorrect = eventService.IsTimeZoneValid(timeZoneString);

        //Assert
        Assert.False(timeZOneIsCorrect);
    }
}