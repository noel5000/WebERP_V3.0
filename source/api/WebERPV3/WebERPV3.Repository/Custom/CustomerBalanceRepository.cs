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
    public class CustomerBalanceRepository : Repository<CustomerBalance>, ICustomerBalanceRepository
    {
        public CustomerBalanceRepository(MainContext context) : base(context)
        {
        }

        public async Task<CustomerBalance> CustomerBalanceByCurrency(long customerId, long currencyId)
        {
            return await _Context.CustomersBalance.AsNoTracking().FirstOrDefaultAsync(x => x.IsDeleted == false && x.CurrencyId == currencyId && x.CustomerId == customerId);
        }
    }
}
