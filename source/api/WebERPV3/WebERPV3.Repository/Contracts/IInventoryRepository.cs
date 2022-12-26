using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IInventoryRepository : IBase<Inventory>
    {
        Task<IEnumerable<Inventory>> GetWarehouseInventoryByProduct(long warehouseId, long productId);
        Task<IEnumerable<Inventory>> GetWarehouseInventory(long warehouseId);
        Task<IEnumerable<Inventory>> GetProductInventory(long productId);
        Task<IEnumerable<Inventory>> GetBranchOfficeInventory(long branchOfficeId, long? productId);
    }
}
