using DatabaseSampler.Application.Messages;
using DatabaseSampler.Application.Services;
using DatabaseSampler.Application.Tests.TestHttpFactories;

namespace DatabaseSampler.Application.Tests;

public class LocationTests
{
    [Fact]
    public async Task Lookup_Postcode_Returns_Expected_Result()
    {
        var responseData = new PostcodeLookupResultData
        {
            Postcode = "SW1A 2AA",
            Latitude = 51.50354,
            Longitude = -0.127695,
            Country = "England",
            Region = "London",
            Outcode = "SW1A",
            //AdminDistrict = "Westminster",
            AdminCounty = null,
            Codes = new PostcodeLookupLocationCodesData
            {
                AdminDistrict = "E09000033",
                AdminCounty = "E99999999",
                AdminWard = "E05000644",
                Parish = "E43000236",
                ParliamentaryConstituency = "E14000639",
                Ccg = "E38000031",
                Ced = "E99999999",
                Nuts = "UKI32"
            }
        };

        var httpClient = new PostcodesTestHttpClientFactory()
            .Get("SW1A 2AA", true, responseData,
                false, null);

        var service = new LocationService(httpClient);

        //https://api.postcodes.io/postcodes/SW1A%202AA

        var result = await service.LookupPostcodeAsync("SW1A 2AA");

        result.Should().NotBeNull();

        result.Postcode.Should().Be("SW1A 2AA");
        result.Latitude.Should().Be(51.50354);
        result.Longitude.Should().Be(-0.127695);
        result.DistrictCode.Should().Be("E09000033");
        result.IsTerminated.Should().BeFalse();
        result.TerminatedYear.Should().BeNull();
        result.TerminatedMonth.Should().BeNull();
    }

    [Fact]
    public async Task Lookup_Terminated_Postcode_Returns_Expected_Result()
    {
        //https://api.postcodes.io/terminated_postcodes/AB1%200AX

        var terminatedPostcodeResponseData = new TerminatedPostcodeLookupResultData
        {
            Postcode = "AB1 0AX",
            Latitude = 57.095679,
            Longitude = -2.24788,
            TerminatedYear = "1996",
            TerminatedMonth = "6"
        };

        var httpClient = new PostcodesTestHttpClientFactory()
            .Get("AB1 0AX", false, null,
                true, terminatedPostcodeResponseData);

        var service = new LocationService(httpClient);

        var result = await service.LookupPostcodeAsync("AB1 0AX");

        result.Should().NotBeNull();

        result.Postcode.Should().Be("AB1 0AX");
        result.Latitude.Should().Be(57.095679);
        result.Longitude.Should().Be(-2.24788);
        result.DistrictCode.Should().BeNull();
        result.IsTerminated.Should().BeTrue();
        result.TerminatedYear.Should().Be(1996);
        result.TerminatedMonth.Should().Be(6);
    }
}
