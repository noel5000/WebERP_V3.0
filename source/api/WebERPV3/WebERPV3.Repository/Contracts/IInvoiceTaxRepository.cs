using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IInvoiceTaxRepository: IBase<InvoiceTax>
    {
        Task<IEnumerable<InvoiceTax>> GetInvoiceTaxes(string invoiceNumber);
        Task<IEnumerable<InvoiceTax>> GetInvoiceTaxes(long invoiceID);
    }
}
