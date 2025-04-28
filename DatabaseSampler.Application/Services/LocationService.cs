using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Messages;
using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Services;

public class LocationService : ILocationService
{
    //TODO: Consider adding this to config
    public const string PostcodeRetrieverBaseUrl = "https://api.postcodes.io/";

    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public LocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _baseUrl = PostcodeRetrieverBaseUrl.TrimEnd('/');
    }

    public async Task<Location> LookupPostcodeAsync(string postcode)
    {
        //https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/

        //Postcodes.io Returns 404 for "CV12 wt" so I have removed all special characters to get best possible result
        var lookupUrl = $"{_baseUrl}/postcodes/{Uri.EscapeDataString(postcode)}";

        var responseMessage = await _httpClient.GetAsync(lookupUrl);

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            //Try for a terminated postcode
            return await LookupTerminatedPostcodeAsync(postcode);
        }

        responseMessage.EnsureSuccessStatusCode();

        var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        var response = await JsonSerializer.DeserializeAsync<PostcodeLookupResponse>(contentStream);

       return response != null 
           ? new Location
           {
                Postcode = response.Result.Postcode,
                Latitude = response.Result.Latitude,
                Longitude = response.Result.Longitude,
                DistrictCode = response.Result.Codes.AdminDistrict,
                IsTerminated = false,
                TerminatedYear = null,
                TerminatedMonth = null
            }
            : null;
    }

    public async Task<Location> LookupTerminatedPostcodeAsync(string postcode)
    {
        //Postcodes.io Returns 404 for "CV12 wt" so I have removed all special characters to get best possible result
        var lookupUrl = $"{_baseUrl}/terminated_postcodes/{Uri.EscapeDataString(postcode)}";

        var responseMessage = await _httpClient.GetAsync(lookupUrl);

        responseMessage.EnsureSuccessStatusCode();

        var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        var response = await JsonSerializer.DeserializeAsync<TerminatedPostcodeLookupResponse>(contentStream);

        return response != null
            ? new Location
            {
                Postcode = response.Result.Postcode,
                Latitude = response.Result.Latitude,
                Longitude = response.Result.Longitude,
                IsTerminated = true,
                TerminatedYear = short.Parse(response.Result.TerminatedYear),
                TerminatedMonth = short.Parse(response.Result.TerminatedMonth)
            }
            : null;
    }
}
