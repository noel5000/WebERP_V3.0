using Microsoft.EntityFrameworkCore;
using WebERPV3.Common;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class ProductTaxRepository : Repository<ProductTax>, IProductTaxRepository
    {
        public ProductTaxRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductTax>> GetProductTaxes(long productId)
        {
            return await _Context.ProductTaxes.AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == productId).ToListAsync();
        }
    }
}
