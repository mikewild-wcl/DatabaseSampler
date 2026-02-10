using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface ICosmosDbService
{
    Task AddExpenseAsync(Expense expense);

    Task<IList<Expense>> GetItemsAsync(string queryString);
}
