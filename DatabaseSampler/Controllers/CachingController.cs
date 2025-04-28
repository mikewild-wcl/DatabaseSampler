using System.Diagnostics;
using System.Text.Json;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace DatabaseSampler.Controllers
{
    [AllowAnonymous]
    public class CachingController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILocationService _locationService;

        public CachingController(IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILocationService locationService)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _locationService = locationService;
        }

        public IActionResult Index()
        {
            return View(new PostcodeLookupViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> LookupPostcode(PostcodeLookupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), viewModel);
            }

            var stopwatch = Stopwatch.StartNew();
            var result = await _locationService.LookupPostcodeAsync(viewModel.Postcode);
            stopwatch.Stop();

            viewModel.Postcode = result.Postcode;
            viewModel.ElapsedTimeForLookup = stopwatch.Elapsed;

            return View("Index", ViewModelFromResult(result, stopwatch.Elapsed));
        }

        [HttpPost]
        public async Task<IActionResult> LookupPostcodeWithMemoryCaching(PostcodeLookupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), viewModel);
            }

            viewModel.UseCachedResponse = true;
            var foundInCache = true;

            var stopwatch = Stopwatch.StartNew();

            var cacheKey = $"Postcode_{viewModel.Postcode.Replace(" ", "")}";

            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out Location result))
            {
                foundInCache = false;
                result = await _locationService.LookupPostcodeAsync(viewModel.Postcode);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _memoryCache.Set(cacheKey, result, cacheEntryOptions);
            }

            stopwatch.Stop();

            //viewModel.Postcode = result.Postcode;
            //viewModel.IsTerminated = result.IsTerminated;
            //viewModel.Latitude = result.Latitude;
            //viewModel.Longitude = result.Longitude;
            //viewModel.FoundInCache = foundInCache;
            //viewModel.ElapsedTimeForLookup = stopwatch.Elapsed;

            return View("Index", ViewModelFromResult(result, stopwatch.Elapsed, foundInCache));
        }

        [HttpPost]
        public async Task<IActionResult> LookupPostcodeWithDistributedCaching(PostcodeLookupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), viewModel);
            }

            //https://dotnetcoretutorials.com/2017/01/06/using-redis-cache-net-core/

            viewModel.UseCachedResponse = true;
            var foundInCache = true;

            var stopwatch = Stopwatch.StartNew();

            var cacheKey = $"Postcode_{viewModel.Postcode.Replace(" ", "")}";

            Location result;
            var serializedLocation = await _distributedCache.GetStringAsync(cacheKey);

            //Need to serialize to binary or string...
            //https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-3.1

            if (serializedLocation != null)
            {
                result = JsonSerializer.Deserialize<Location>(serializedLocation);
            }
            else
            {
                foundInCache = false;
                result = await _locationService.LookupPostcodeAsync(viewModel.Postcode);

                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                serializedLocation = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(cacheKey, serializedLocation, cacheEntryOptions);
            }

            stopwatch.Stop();

            //viewModel.Postcode = result.Postcode;
            //viewModel.FoundInCache = foundInCache;
            //viewModel.ElapsedTimeForLookup = stopwatch.Elapsed;

            return View("Index", ViewModelFromResult(result, stopwatch.Elapsed, foundInCache));
        }

        private PostcodeLookupViewModel ViewModelFromResult(Location result, TimeSpan? lookupTime, bool foundInCache = false)
        {
            var viewModel = new PostcodeLookupViewModel
            {
                Postcode = result.Postcode,
                IsTerminated = result.IsTerminated,
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                FoundInCache = foundInCache,
                ElapsedTimeForLookup = lookupTime,
            };
            return viewModel;
        }
    }
}
