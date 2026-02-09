using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Functions.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Web;
using FunctionsWorkerHttp = Microsoft.Azure.Functions.Worker.Http;

namespace DatabaseSampler.Functions;

public class LocationFunctions(
    ILocationService locationService,
    ILogger<LocationFunctions> logger)
{
    private readonly ILocationService _locationService = locationService;
    private readonly ILogger<LocationFunctions> _logger = logger;

    private static readonly Action<ILogger, string, Exception?> _logGetFunctionTriggered =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(0, nameof(StudentFunctions)),
            "GET location HTTP trigger function called with postcode '{Postcode}'.");

    private static readonly Action<ILogger, string?, Exception?> _logPostFunctionTriggered =
        LoggerMessage.Define<string?>(
            LogLevel.Information,
            new EventId(0, nameof(StudentFunctions)),
            "POST location HTTP trigger function called with postcode '{Postcode}'.");

    [Function("GetLocation")]
    public async Task<IActionResult> GetLocation(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "location/{postcode}")]
        HttpRequest _,
        string postcode)
    {
        _logGetFunctionTriggered(_logger, postcode, null);

        if(string.IsNullOrEmpty(postcode))
        {
            return new BadRequestObjectResult("Postcode is required in the body of the request");
        }

        postcode = HttpUtility.UrlDecode(postcode);

        var data = await _locationService.LookupPostcodeAsync(postcode);

        return new OkObjectResult(data);
    }

    [Function("PostLocation")]
    public async Task<IActionResult> PostLocation(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "location")]
        HttpRequest _,
        [FunctionsWorkerHttp.FromBody]
        PostcodeRequest payload,
        CancellationToken cancellationToken)
    {
        var postcode = payload?.Postcode;

        _logPostFunctionTriggered(_logger, postcode, null);

        if (string.IsNullOrEmpty(postcode))
        {
            return new BadRequestObjectResult("Postcode is required in the body of the request");
        }

        var data = await _locationService.LookupPostcodeAsync(postcode);

        return new OkObjectResult(data);
    }
}
