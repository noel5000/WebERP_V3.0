using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class UnitProductEquivalenceRepository : Repository<UnitProductEquivalence>, IUnitProductEquivalenceRepository
    {
        public UnitProductEquivalenceRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UnitProductEquivalence>> GetProductUnits(long productId)
        {
            return await Task.Factory.StartNew<IEnumerable<UnitProductEquivalence>>(() => {
                throw new NotImplementedException();
            });
        }
    }
}
