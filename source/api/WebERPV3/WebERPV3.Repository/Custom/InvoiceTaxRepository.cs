using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class InvoiceTaxRepository : Repository<InvoiceTax>, IInvoiceTaxRepository
    {
        public InvoiceTaxRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InvoiceTax>> GetInvoiceTaxes(string invoiceNumber)
        {
            return await _Context.InvoicesTaxes.AsNoTracking().Where(x => x.InvoiceNumber.ToLower() == invoiceNumber.ToLower()).ToListAsync();
        }

    public async Task<IEnumerable<InvoiceTax>> GetInvoiceTaxes(long invoiceID)
        {
            return await _Context.InvoicesTaxes.AsNoTracking().Where(x => x.InvoiceId == invoiceID).ToListAsync();
        }
    }
}
