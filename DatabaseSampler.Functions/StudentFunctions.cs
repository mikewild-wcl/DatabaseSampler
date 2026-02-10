using Microsoft.Extensions.Logging;
using DatabaseSampler.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Functions;

public class StudentFunctions(
    IPostgresSqlService postgresSqlService,
    ILogger<StudentFunctions> logger)
{
    private readonly IPostgresSqlService _postgresSqlService = postgresSqlService;
    private readonly ILogger<StudentFunctions> _logger = logger;

    private static readonly Action<ILogger, Exception?> _logFunctionTriggered = 
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(810, nameof(StudentFunctions)),
            "Get students HTTP trigger function called.");

    [Function("GetStudents")]
    public async Task<IActionResult> GetStudents(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "students")]
        HttpRequest _)
    {
        _logFunctionTriggered(_logger, null);

        var data = await _postgresSqlService.GetStudentsAsync();

        return new OkObjectResult(data);
    }
}
