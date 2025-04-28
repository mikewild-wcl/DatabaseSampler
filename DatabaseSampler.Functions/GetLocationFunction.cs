using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DatabaseSampler.Functions
{
    public class GetLocationFunction
    {
        private readonly ILocationService _locationService;

        public GetLocationFunction(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [Function("GetLocation")]
        public async Task<HttpResponseData> GetLocation(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HttpFunction");
            logger.LogInformation("GetLocation HTTP trigger function processed a request.");

            var method = req.Method;//.Query["postcode"];
            logger.LogInformation($"Method is {method}");

            string postcode = null;
            switch (method)
            {
                case "GET":
                    var uri = req.Url;//.Query["postcode"];
                    logger.LogInformation($"Uri is {uri}");
                    logger.LogInformation($"Uri query is is {uri.Query}");
                    var queryParameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    postcode = queryParameters["postcode"];
                    break;

                case "POST":
                    using (var streamReader = new StreamReader(req.Body))
                    {
                        var payload = await streamReader.ReadToEndAsync();
                        logger.LogInformation($"Have payload {payload}");
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
}
