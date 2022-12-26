using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IExpenseTaxRepository: IBase<ExpenseTax>
    {
        Task<IEnumerable<ExpenseTax>> GetExpenseTaxes(string reference);
       Task<IEnumerable<ExpenseTax>> GetExpenseTaxes(long id);
    }
}
