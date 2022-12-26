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
    public class WarehouseMovementRepository : Repository<WarehouseMovement>, IWarehouseMovementRepository
    {
        public WarehouseMovementRepository(MainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WarehouseMovement>> GetMovementsByProduct(long? productId, long? warehouseId, DateTime? initialDate, DateTime? endDate)
        {
            return await _Context.WarehousesMovements.AsNoTracking().Where(x => x.IsDeleted == false &&
            (warehouseId.HasValue ? x.WarehouseId == warehouseId.Value : x.WarehouseId > 0) &&
             (initialDate.HasValue ? x.CreatedDate >= initialDate.Value : x.CreatedDate > DateTime.MinValue) &&
             (endDate.HasValue ? x.CreatedDate <= endDate.Value : x.CreatedDate< DateTime.Now) &&
            (productId.HasValue ? x.ProductId == productId.Value : x.ProductId > 0)).ToListAsync();
        }

    public async Task<IEnumerable<WarehouseMovement>> GetProductHistory(long? productId)
        {
            return await _Context.WarehousesMovements.AsNoTracking().Where(x => x.IsDeleted == false &&
            (productId.HasValue ? x.ProductId == productId.Value : x.ProductId > 0)).ToListAsync();
        }
    }
}
