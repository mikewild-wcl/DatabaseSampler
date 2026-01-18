using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Controllers;

[AllowAnonymous]
public class PostgresController : Controller
{
    private readonly IDataGenerator _dataGenerator;
    private readonly IPostgresSqlService _postgresSqlService;

    public PostgresController(IDataGenerator dataGenerator, IPostgresSqlService postgresSqlService)
    {
        _dataGenerator = dataGenerator;
        _postgresSqlService = postgresSqlService;
    }

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
        
        Console.WriteLine($"Created student with id {id} for {student.FirstName} {student.LastName}");

        var vm = await LoadStudentsViewModelAsync();

        return View("Index", vm);
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
