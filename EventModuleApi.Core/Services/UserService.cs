using EvenrModule.Persistence.Models.UserDetails;
using EventModuleApi.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace EventModuleApi.Core.Services;

public class UserService : IUserService
{
    private HttpClient Client { get; }
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(HttpClient client, IConfiguration configuration, ILogger<UserService> logger)
    {
        _configuration = configuration;

        var baseUrl = _configuration["UserDetails:BaseUrl"];
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        client.Timeout = TimeSpan.FromSeconds(3);

        Client = client;
        _logger = logger;
    }

    public async Task<User?> GetUserDetailsAsync(int userId)
    {
        try
        {
            //make the http request
            var response = await Client.GetAsync($"users/{userId}");

            //read the response stream
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(responseStream);
            var responseValue = await reader.ReadToEndAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Deserialize the response
                return JsonSerializer.Deserialize<User>(responseValue);
            }
            else
            {
                _logger.LogError($"GetUserDetails for userid {userId} returned failed");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetUserDetailsAsync message = " + ex.Message + " \nStack trace = " + ex.StackTrace);
            return null;
        }
    }
}
