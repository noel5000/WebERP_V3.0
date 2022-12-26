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
    public class InvoiceDetailRepository : Repository<InvoiceDetail>, IInvoiceDetailRepository
    {
        public InvoiceDetailRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByInvoiceId(long invoiceId)
        {
            return await _Context.InvoiceDetails.AsNoTracking().Where(x => x.IsDeleted == false && x.InvoiceId == invoiceId).ToListAsync();
        }

    public async Task<IEnumerable<InvoiceDetail>> GetByProductId(long productId)
        {
            return await _Context.InvoiceDetails.AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == productId).ToListAsync();
        }

public async Task<IEnumerable<InvoiceDetail>> GetChildren(long parentId)
        {
            return await _Context.InvoiceDetails.AsNoTracking().Where(x => x.IsDeleted == false && x.ParentId==parentId).ToListAsync();
        }

public async Task<IEnumerable<InvoiceDetail>> GetInvoiceParentsDetails(long invoiceId)
        {
            return await _Context.InvoiceDetails.AsNoTracking().Where(x => x.IsDeleted == false && x.InvoiceId == invoiceId && x.ParentId==null).ToListAsync();
        }
    }
}
