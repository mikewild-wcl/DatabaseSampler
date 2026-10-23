using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Controllers;

[AllowAnonymous]
public partial class PostgresController(
        IPostgresSqlService postgresSqlService,
        IDataGenerator dataGenerator,
        ILogger<PostgresController> logger) : Controller
{
    private readonly IPostgresSqlService _postgresSqlService = postgresSqlService;
    private readonly IDataGenerator _dataGenerator = dataGenerator;
    private readonly ILogger<PostgresController> _logger = logger;

    [LoggerMessage(
        EventId = 301,
        Level = LogLevel.Information,
        Message = "Student created with Id={Id}, Name {FirstName} {LastName}.")]
    private static partial void LogStudentCreated(ILogger logger, int id, string firstName, string lastName);

    public async Task<IActionResult> Index()
    {
        var vm = await LoadStudentsViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent()
    {
        // How to Configure PostgreSQL in Entity Framework Core – Code Maze
        //https://code--maze-com.cdn.ampproject.org/v/s/code-maze.com/configure-postgresql-ef-core/amp/?usqp=mq331AQCKAE%3D&amp_js_v=0.1

        var student = _dataGenerator.CreateStudent();
        var id = await _postgresSqlService.AddStudentAsync(student);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogStudentCreated(_logger, id, student.FirstName, student.LastName);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<StudentsViewModel> LoadStudentsViewModelAsync()
    {
        var students = await _postgresSqlService.GetStudentsAsync();
        return new StudentsViewModel
        {
            Students = students
        };
    }
}
