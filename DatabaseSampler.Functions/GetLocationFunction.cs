using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DatabaseSampler.Functions;

public class GetLocationFunction(
    ILocationService locationService,
    ILogger<GetLocationFunction> logger)
{
    private readonly ILocationService _locationService = locationService;
    private readonly ILogger<GetLocationFunction> _logger = logger;

    [Function("GetLocation")]
    public async Task<HttpResponseData> GetLocation(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "location")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("GetLocation HTTP trigger function processed a request.");

        var method = req.Method;//.Query["postcode"];
        _logger.LogInformation($"Method is {method}");

        string postcode = null;
        switch (method)
        {
            case "GET":
                var uri = req.Url;//.Query["postcode"];
                _logger.LogInformation($"Uri is {uri}");
                _logger.LogInformation($"Uri query is is {uri.Query}");
                var queryParameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
                postcode = queryParameters["postcode"];
                break;

            case "POST":
                using (var streamReader = new StreamReader(req.Body))
                {
                    var payload = await streamReader.ReadToEndAsync();
                    _logger.LogInformation($"Have payload {payload}");
                    postcode = payload;
                }
                break;
        }

        var data = await _locationService.LookupPostcodeAsync(postcode);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var json = JsonSerializer.Serialize(data);
        await response.WriteStringAsync(json);

        return response;
    }
}
