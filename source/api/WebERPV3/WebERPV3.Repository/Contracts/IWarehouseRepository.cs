using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IWarehouseRepository: IBase<Warehouse>
    {
        Task<IEnumerable<Inventory>> GetProductInventory(long productId);
    }
}
