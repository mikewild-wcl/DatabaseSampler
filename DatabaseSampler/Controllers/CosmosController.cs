using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Controllers
{
    [AllowAnonymous]
    public class CosmosController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        
        public CosmosController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new CosmosItemsViewModel
            {
                Expenses = await _cosmosDbService
                    .GetItemsAsync("SELECT * FROM c")
            };

            return View(vm);
        }
    }
}
