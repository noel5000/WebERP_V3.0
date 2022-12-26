using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IInvoiceRepository : IBase<Invoice>
    {
        Task<IEnumerable<Invoice>> GetAccountsReceivable(DateTime? startDate, DateTime? endDate, Nullable<long> customerId, Nullable<long> currencyId, long? sellerId);
        Task<IEnumerable<Invoice>> GetSales(DateTime? startDate, DateTime? endDate, Nullable<long> customerId, Nullable<long> currencyId, Nullable<long> sellerId);
        Task<Invoice> GetByInvoiceNumber(string invoiceNumber);
        PagedList<Invoice> GetPagedQuotes(int page, int size);
        Task<IEnumerable<object>> GetAccountStatus(DateTime? startDate, DateTime? endDate, long? customerId, long? currencyId);

    }
}
