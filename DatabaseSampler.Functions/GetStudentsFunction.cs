using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DatabaseSampler.Functions;

public class GetStudentsFunction
{
    private readonly IPostgresSqlService _postgresSqlService;

    public GetStudentsFunction(IPostgresSqlService postgresSqlService)
    {
        _postgresSqlService = postgresSqlService;
    }

    [Function("GetStudents")]
    public async Task<HttpResponseData> GetStudents(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] 
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HttpFunction");
        logger.LogInformation("GetStudents HTTP trigger function processed a request.");
        
        var data = await _postgresSqlService.GetStudentsAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var json = JsonSerializer.Serialize(data);
        await response.WriteStringAsync(json);

        return response;
    }
}
