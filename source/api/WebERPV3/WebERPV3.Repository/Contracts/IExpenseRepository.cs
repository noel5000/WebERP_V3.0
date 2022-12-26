using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IExpenseRepository : IBase<Expense>
    {
        Task<IEnumerable<Expense>> GetDebsToPay(DateTime? startDate, DateTime? endDate, Nullable<long> supplierId, Nullable<long> currencyId, Nullable<long> branchOfficeId);
    }
}
