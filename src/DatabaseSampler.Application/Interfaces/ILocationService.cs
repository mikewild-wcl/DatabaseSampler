using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface ILocationService
{
    Task<Location> LookupPostcodeAsync(string postcode);
}
