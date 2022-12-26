using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ICompositeProductRepository: IBase<CompositeProduct>
    {
        Task<IEnumerable<CompositeProduct>> GetDerivedProducts(long productId);
        Task<IEnumerable<CompositeProduct>> GetProductBases(long productId);
    }
}
