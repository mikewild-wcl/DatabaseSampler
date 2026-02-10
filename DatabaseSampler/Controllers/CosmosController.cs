using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSampler.Controllers;

[AllowAnonymous]
public partial class CosmosController(
    ICosmosDbService cosmosDbService,
    IDataGenerator dataGenerator,
    ILogger<CosmosController> logger) : Controller
{
    private readonly ICosmosDbService _cosmosDbService = cosmosDbService;
    private readonly IDataGenerator _dataGenerator = dataGenerator;
    private readonly ILogger<CosmosController> _logger = logger;

    [LoggerMessage(
        EventId = 201,
        Level = LogLevel.Information,
        Message = "Expense created with Id {Id} Name {Name}, Amount {Amount}, MonthlyCost {MonthlyCost}, Frequency {Frequency}, StartDate {StartDate}.")]
    private static partial void LogExpenseCreated(ILogger logger, Guid Id, string name, decimal amount, decimal monthlyCost, string frequency, DateTime startDate);

    public async Task<IActionResult> Index()
    {
        var vm = new CosmosItemsViewModel
        {
            Expenses = await _cosmosDbService
                .GetItemsAsync("SELECT * FROM c")
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense()
    {
        var expense = _dataGenerator.CreateExpense();

        await _cosmosDbService.AddExpenseAsync(expense);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogExpenseCreated(_logger, expense.Id, expense.Name, expense.Amount, expense.MonthlyCost, expense.Frequency, expense.StartDate);
        }

        return RedirectToAction(nameof(Index));
    }
}
