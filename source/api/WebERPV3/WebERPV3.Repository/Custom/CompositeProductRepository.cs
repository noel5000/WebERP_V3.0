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
    public class CompositeProductRepository : Repository<CompositeProduct>, ICompositeProductRepository
    {
        public CompositeProductRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CompositeProduct>> GetDerivedProducts(long productId)
        {
            return await base._Context.CompositeProducts.AsNoTracking().Where(x => x.IsDeleted == false && x.BaseProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<CompositeProduct>> GetProductBases(long productId)
        {
            return await _Context.CompositeProducts.AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == productId).ToListAsync();
        }
    }
}
