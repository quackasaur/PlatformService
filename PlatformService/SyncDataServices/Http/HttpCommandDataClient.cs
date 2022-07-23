using PlatformService.Dtos;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
  public class HttpCommandDataClient : ICommandDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _configuration = configuration;
    }
    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
      var httpContent = new StringContent(JsonSerializer.Serialize(platform), System.Text.Encoding.UTF8, "application/json");

      var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/c/platforms", httpContent);

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("Got the response from the Command service");
      }

      else
      {
        Console.WriteLine("Something went wrong with the response");
      }
    }
  }
}
