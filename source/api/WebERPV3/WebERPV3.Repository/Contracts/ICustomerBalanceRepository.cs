using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ICustomerBalanceRepository: IBase<CustomerBalance>
    {
        Task<CustomerBalance> CustomerBalanceByCurrency(long customerId, long currencyId);
    }
}
