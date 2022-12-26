using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetProductInventory(long productId)
        {
            return await Task.Factory.StartNew<IEnumerable<Inventory>>(() => {
                throw new NotImplementedException();
            });
        }
    }
}
