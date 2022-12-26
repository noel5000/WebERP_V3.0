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
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetBranchOfficeInventory(long branchOfficeId, long? productId)
        {
          

            return await _Context.Inventory.AsNoTracking().Where(
                inventory => inventory.IsDeleted == false && (productId.HasValue ? inventory.ProductId == productId.Value : inventory.Id > 0) &&
            inventory.BranchOfficeId == branchOfficeId
                ).ToListAsync();
        }

    public async Task<IEnumerable<Inventory>> GetProductInventory(long productId)
        {
            return await _Context.Inventory.AsNoTracking().Where(x=>x.IsDeleted ==false && x.ProductId==productId).ToListAsync();
        }

public async Task<IEnumerable<Inventory>> GetWarehouseInventory(long warehouseId)
        {
            return await _Context.Inventory.AsNoTracking().Where(x => x.IsDeleted == false && x.WarehouseId == warehouseId).ToListAsync();
        }

public async Task<IEnumerable<Inventory>> GetWarehouseInventoryByProduct(long warehouseId, long productId)
        {
            return await _Context.Inventory.AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == productId && x.WarehouseId==warehouseId).ToListAsync();
        }
    }
}
