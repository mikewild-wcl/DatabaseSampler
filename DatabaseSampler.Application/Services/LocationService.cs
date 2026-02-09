using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Messages;
using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Services;

public class LocationService : ILocationService
{
    public const string PostcodeRetrieverBaseUrl = "https://api.postcodes.io/";

    private readonly HttpClient _httpClient;

    public LocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<Location> LookupPostcodeAsync(string postcode)
    {
        //Postcodes.io Returns 404 for "CV12 wt" so I have removed all special characters to get best possible result
        var lookupUrl = $"postcodes/{Uri.EscapeDataString(postcode)}";

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
        //Postcodes.io Returns 404 for "CV12 wt" so I have removed special characters to get the best match
        var lookupUrl = $"terminated_postcodes/{Uri.EscapeDataString(postcode)}";

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
