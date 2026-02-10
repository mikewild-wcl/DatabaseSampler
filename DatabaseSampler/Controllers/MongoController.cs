using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Controllers;

[AllowAnonymous]
public partial class MongoController(
    IMongoDbService mongoDbService,
    IDataGenerator dataGenerator,
    ILogger<MongoController> logger) : Controller
{
    private readonly IDataGenerator _dataGenerator = dataGenerator;
    private readonly IMongoDbService _mongoDbService = mongoDbService;
    private readonly ILogger<MongoController> _logger = logger;

    [LoggerMessage(
        EventId = 502,
        Level = LogLevel.Information,
        Message = "Teacher created with Id={Id}, Name {FirstName} {LastName}, Subject {Subject}, Joined {Joined}.")]
    private static partial void LogTeacherCreated(ILogger logger, int id, string firstName, string lastName, string subject, DateTime joined);

    public async Task<IActionResult> Index()
    {
        var vm = new MongoItemsViewModel
        {
            Teachers = await _mongoDbService
                .GetTeachersAsync("SELECT * FROM c")
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeacher()
    {
        var teacher = _dataGenerator.CreateTeacher();
        //var id = await _mongoDbService.AddTeacherAsync(teacher);
        var id = -1;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogTeacherCreated(_logger, teacher.Id, teacher.FirstName, teacher.LastName, teacher.SpecialistSubject, teacher.Joined);
        }

        return RedirectToAction(nameof(Index));
    }
}
