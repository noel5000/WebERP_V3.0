using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IExpensesPaymentRepository : IBase<ExpensesPayment>
    {
        Task<Result<Payment>> AddPayment(CompanyPayments payment, List<Expense> expenses); 
    }
}
