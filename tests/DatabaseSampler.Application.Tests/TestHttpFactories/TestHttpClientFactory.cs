using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DatabaseSampler.Application.Tests.TestHttpFactories;

public abstract class TestHttpClientFactory
{
    // ReSharper disable once UnusedMember.Global
    protected HttpClient CreateClient(string uri, object response, string contentType = "application/json")
    {
        return CreateClient(
        [
            new()
            {
                Uri =uri,
                ResponseObject = response
            }
        ]);
    }

    protected HttpClient CreateClient(IEnumerable<RequestWrapper> requests, string contentType = "application/json")
    {
        var baseAddress = new Uri("https://api.postcodes.io/");

        var fakeMessageHandler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(fakeMessageHandler)
        {
            BaseAddress = baseAddress
        };

        foreach (var request in requests)
        {
            var serialized = JsonSerializer.Serialize(request.ResponseObject);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serialized)
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            fakeMessageHandler.AddFakeResponse(new Uri(baseAddress, request.Uri),
                httpResponseMessage);
        }

        return httpClient;
    }
}