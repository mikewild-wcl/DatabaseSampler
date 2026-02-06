using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DatabaseSampler.Functions;

public class GetStudentsFunction(
    IPostgresSqlService postgresSqlService,
    ILogger<GetStudentsFunction> logger)
{
    private readonly IPostgresSqlService _postgresSqlService = postgresSqlService;
    private readonly ILogger<GetStudentsFunction> _logger = logger;

    private static readonly Action<ILogger, Exception?> _logFunctionTriggered =
    LoggerMessage.Define(
        LogLevel.Information,
        new EventId(0, nameof(GetStudentsFunction)),
        "Get students HTTP trigger function called.");

    [Function("GetStudents")]
    public async Task<HttpResponseData> GetStudents(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "students")] 
        HttpRequestData req)
    {
        _logFunctionTriggered(_logger, null);
        
        var data = await _postgresSqlService.GetStudentsAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var json = JsonSerializer.Serialize(data);
        await response.WriteStringAsync(json);

        return response;
    }
}
