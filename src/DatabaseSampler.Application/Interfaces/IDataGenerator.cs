using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IDataGenerator
{
    Expense CreateExpense();

    Student CreateStudent();

    Teacher CreateTeacher();
}
