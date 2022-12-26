using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class ExpenseTaxRepository : Repository<ExpenseTax>, IExpenseTaxRepository
    {
        public ExpenseTaxRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExpenseTax>> GetExpenseTaxes(string reference)
        {
            return await _Context.ExpenseTaxes.AsNoTracking().Where(x => x.IsDeleted == false && x.Reference.ToLower() == reference.ToLower()).ToListAsync();
        }

    public async Task<IEnumerable<ExpenseTax>> GetExpenseTaxes(long id)
        {
            return await _Context.ExpenseTaxes.AsNoTracking().Where(x => x.IsDeleted == false && x.ExpenseId==id).ToListAsync();
        }
    }
}
