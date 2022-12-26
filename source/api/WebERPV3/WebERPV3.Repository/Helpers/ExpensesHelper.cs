using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers
{
   public class ExpensesHelper
    {
      
        public static async  Task UpdateExpenseTaxes(Expense expense, IDataRepositoryFactory dataRepositoryFactory)
        {
            var expensesTaxRepo = dataRepositoryFactory.GetCustomDataRepositories<IExpenseTaxRepository>();
            expense.Taxes = expense.Taxes == null ? new List<ExpenseTax>() : expense.Taxes;
            var expenseTaxes = (await expensesTaxRepo.GetExpenseTaxes(expense.ExpenseReference)).ToList();
            if (expenseTaxes != null && expenseTaxes.Count > 0)
            {
                foreach (ExpenseTax tax in expenseTaxes)
                {
                  await  expensesTaxRepo.Remove(tax.Id);
                }
            }



            foreach (var expenseTax in expense.Taxes)
            {
                ExpenseTax tax = new ExpenseTax()
                {
                   CreatedBy = expense.CreatedBy,
                    IsDeleted = false,
                    ExpenseId = expense.Id,
                    Date = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    TaxId = expenseTax.TaxId,
                    CurrencyId = expense.CurrencyId,
                    TaxAmount = expenseTax.TaxAmount,
                    Reference = expense.ExpenseReference,

                };
               await expensesTaxRepo.Add(tax);
            }
        }

        public static async Task< Result<ExpensesPayment>> ApplyExpensePayment(ExpensesPayment payment, IDataRepositoryFactory dataRepositoryFactory)
        {
            var paymentRepo=dataRepositoryFactory.GetDataRepositories<ExpensesPayment>();
            return await paymentRepo.Add(payment);
        }

    }
}
