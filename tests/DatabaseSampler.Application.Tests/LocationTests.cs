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

        using var httpClient = new PostcodesTestHttpClientFactory()
            .Get("SW1A 2AA", true, responseData,
                false, null);

        var service = new LocationService(httpClient);

        //https://api.postcodes.io/postcodes/SW1A%202AA

        var result = await service.LookupPostcodeAsync("SW1A 2AA");

        result.ShouldNotBeNull();

        result.Postcode.ShouldBe("SW1A 2AA");
        result.Latitude.ShouldBe(51.50354);
        result.Longitude.ShouldBe(-0.127695);
        result.DistrictCode.ShouldBe("E09000033");
        result.IsTerminated.ShouldBeFalse();
        result.TerminatedYear.ShouldBeNull();
        result.TerminatedMonth.ShouldBeNull();
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

        using var httpClient = new PostcodesTestHttpClientFactory()
            .Get("AB1 0AX", false, null,
                true, terminatedPostcodeResponseData);

        var service = new LocationService(httpClient);

        var result = await service.LookupPostcodeAsync("AB1 0AX");

        result.ShouldNotBeNull();

        result.Postcode.ShouldBe("AB1 0AX");
        result.Latitude.ShouldBe(57.095679);
        result.Longitude.ShouldBe(-2.24788);
        result.DistrictCode.ShouldBeNull();
        result.IsTerminated.ShouldBeTrue();
        result.TerminatedYear.ShouldBe((short)1996);
        result.TerminatedMonth.ShouldBe((short)6);
    }
}
