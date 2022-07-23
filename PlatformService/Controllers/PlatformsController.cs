using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
      _repository = repository;
      _mapper = mapper;
      _commandDataClient = commandDataClient;
      _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public IActionResult GetPlatforms()
    {
      var platformItems = _repository.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public IActionResult GetPlatformById(int id)
    {
      var platformItem = _repository.GetPlatformById(id);

      if (platformItem == null)
      {
        return NotFound("Id not found");
      }

      return Ok(_mapper.Map<PlatformReadDto>(platformItem));
    }

    [HttpPost]
    public IActionResult CreatePlatform(PlatformCreateDto platform)
    {
      var platformModel = _mapper.Map<Platform>(platform);

      _repository.CreatePlatform(platformModel);
      _repository.SaveChanges();

      var platformRead = _mapper.Map<PlatformReadDto>(platformModel);
      var platformPublish = _mapper.Map<PlatformPublishDto>(platformRead);
      platformPublish.EventType = "PlatformPublished";
      try
      {
        _messageBusClient.PublishNewPlatform(platformPublish);
      }

      catch (Exception ex)
      {
        Console.WriteLine($"PlatformsController - Something went wrong while sending a message: {ex.Message}");
      }

      return CreatedAtRoute(nameof(GetPlatformById), new { platformRead.Id }, platformRead);
    }
  }
}
