using Microsoft.EntityFrameworkCore;
using WebERPV3.Common;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebERPV3.Common.Enums;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class WarehouseTransferRepository : Repository<WarehouseTransfer>, IWarehouseTransferRepository
    {
        readonly ISequenceManagerRepository sequenceRepo;
        readonly IWarehouseMovementRepository warehouseMovements;
        public WarehouseTransferRepository(MainContext context, IDataRepositoryFactory repositoryFactory) : base(context)
        {
            this.sequenceRepo = repositoryFactory.GetCustomDataRepositories<ISequenceManagerRepository>();
            this.warehouseMovements = repositoryFactory.GetCustomDataRepositories<IWarehouseMovementRepository>();
        }

        public async Task<Result<object>> AddTransfersList(List<WarehouseTransfer> entries, string reference, string details)
        {
            var result = new Result<object>(-1, -1, "error_msg");

            using (var tran = await _Context.Database.BeginTransactionAsync())
            {
                try
                {
                    string sequence = await sequenceRepo.CreateSequence(SequenceTypes.WarehouseTransfers);
                    List<Inventory> inventories = new List<Inventory>();
                    entries.ForEach(async e =>
                    {
                        e.Details = details;
                        e.Date = DateTime.Now;
                        e.Reference = reference;
                        e.Sequence = sequence;

                        int destinyIndex = inventories.FindIndex(x => x.ProductId == e.ProductId && x.WarehouseId == e.DestinyId);
                        int originIndex = inventories.FindIndex(x => x.ProductId == e.ProductId && x.WarehouseId == e.OriginId);
                        e.Product = e.Product != null ? e.Product : new Product() { Id = e.ProductId };
                        e.Product.ProductUnits = await _Context.UnitProductsEquivalences.Include(x => x.Unit).AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == e.ProductId).ToListAsync();
                        var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
            e.Quantity,
            e.UnitId,
              e.Product.ProductUnits.ToList()
            );
                        if (destinyIndex < 0)
                        {
                            inventories.Add(new Inventory()
                            {
                                Quantity = convertionResult.Status >= 0 ? (decimal)convertionResult.Data.FirstOrDefault() : 0,
                                BranchOfficeId = e.DestinyBranchOfficeId,
                                ProductId = e.ProductId,
                                UnitId = e.Product.ProductUnits.FirstOrDefault(x => x.IsPrimary == true).UnitId,
                                WarehouseId = e.DestinyId
                            });
                        }
                        else
                        {
                            inventories[destinyIndex].Quantity += convertionResult.Status >= 0 ? (decimal)convertionResult.Data.FirstOrDefault() : 0;
                        }

                        if (originIndex < 0)
                        {
                            inventories.Add(new Inventory()
                            {
                                Quantity = convertionResult.Status >= 0 ? -1*(decimal)convertionResult.Data.FirstOrDefault() : 0,
                                BranchOfficeId = e.OriginBranchOfficeId,
                                ProductId = e.ProductId,
                                UnitId = e.Product.ProductUnits.FirstOrDefault(x => x.IsPrimary == true).UnitId,
                                WarehouseId = e.OriginId
                            });
                        }
                        else
                        {
                            inventories[originIndex].Quantity += convertionResult.Status >= 0 ?-1* (decimal)convertionResult.Data.FirstOrDefault() : 0;
                        }
                        e.OriginBranchOffice = null;
                        e.Product = null;
                        e.Unit = null;
                        e.DestinyBranchOffice = null;
                        e.Origin = null;
                        e.Destiny = null;
                    });

                  await  _Context.WarehousesTransfers.AddRangeAsync(entries);
                    await _Context.SaveChangesAsync();

                    inventories.ForEach( async inventory =>
                    {
                        if (inventory.Quantity != 0) {
                            var currentInventory = await _Context.Inventory.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == inventory.ProductId && x.WarehouseId == inventory.WarehouseId && x.UnitId == inventory.UnitId && x.IsDeleted == false);
                            if (currentInventory != null)
                            {
                                currentInventory.Quantity += inventory.Quantity;
                                _Context.Inventory.Update(currentInventory);
                            }
                            else
                                 _Context.Inventory.Add(inventory);
                             _Context.SaveChanges();
                            var movement = new WarehouseMovement()
                            {
                                CurrentBalance = currentInventory.Quantity,
                                BranchOfficeId = inventory.BranchOfficeId,
                                MovementType = inventory.Quantity > 0 ? Enums.MovementTypes.IN.ToString() : Enums.MovementTypes.OUT.ToString(),
                                ProductId = inventory.ProductId,
                                Quantity = inventory.Quantity,
                                Reference = sequence,
                                UnitId = inventory.UnitId,
                                WarehouseId = inventory.WarehouseId
                            };
                            await warehouseMovements.Add(movement);
                            
                        }
                       
                    });
                    await tran.CommitAsync();
                    return new Result<object>(0, 0, "ok_msg");
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    result = new Result<object>(-1, -1, "error_msg", null, new Exception(ex.Message));
                }
            }

            return result;
        }

        public async Task<Result<object>> RemoveTransfers(string sequence)
        {
            Result<object> result = new Result<object>(-1, -1, "error_msg");
            using (var tran = await _Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entries = await _Context.WarehousesTransfers.AsNoTracking().Where(x => x.IsDeleted == false && x.Sequence == sequence).ToListAsync();
                    entries.ForEach(async e =>
                    {
                        e.IsDeleted = true;
                        var productUnits = await _Context.UnitProductsEquivalences.AsNoTracking().Where(x => x.IsDeleted == false && x.ProductId == e.ProductId).ToListAsync();
                        var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
e.Quantity,
e.UnitId,
productUnits
);
                        var DestinyInventory = await _Context.Inventory.AsNoTracking().FirstOrDefaultAsync(x => x.IsDeleted == false && x.ProductId == e.ProductId && x.WarehouseId == e.DestinyId);
                        var originInventory = await _Context.Inventory.AsNoTracking().FirstOrDefaultAsync(x => x.IsDeleted == false && x.ProductId == e.ProductId && x.WarehouseId == e.OriginId);
                       
                        DestinyInventory.Quantity -= (decimal)convertionResult.Data.FirstOrDefault();
                        originInventory.Quantity += (decimal)convertionResult.Data.FirstOrDefault();
                        var movements =new List<WarehouseMovement>(){ new WarehouseMovement()
                        {
                            CurrentBalance =  originInventory.Quantity,
                            BranchOfficeId = originInventory.BranchOfficeId,
                            MovementType = Enums.MovementTypes.IN.ToString(),
                            ProductId = originInventory.ProductId,
                            Quantity = (decimal)convertionResult.Data.FirstOrDefault(),
                            Reference = sequence,
                            UnitId = originInventory.UnitId,
                            WarehouseId = originInventory.WarehouseId
                        },
                        new WarehouseMovement()
                        {
                            CurrentBalance = DestinyInventory.Quantity,
                            BranchOfficeId = DestinyInventory.BranchOfficeId,
                            MovementType = Enums.MovementTypes.OUT.ToString(),
                            ProductId = DestinyInventory.ProductId,
                            Quantity =-1* (decimal)convertionResult.Data.FirstOrDefault(),
                            Reference = sequence,
                            UnitId = DestinyInventory.UnitId,
                            WarehouseId = DestinyInventory.WarehouseId
                        }
                        };
                        _Context.Inventory.UpdateRange(new List<Inventory>() 
                        {
                        DestinyInventory,
                        originInventory
                        });
                         _Context.SaveChanges();
                        await warehouseMovements.AddRange(movements);

                    });
                    _Context.WarehousesTransfers.UpdateRange(entries);
                    await _Context.SaveChangesAsync();
                    await tran.CommitAsync();
                    result = new Result<object>(0, 0, "ok_msg");
                }
                catch (Exception ex)
                {
                  await  tran.RollbackAsync();
                    result = new Result<object>(-1, -1, "error_msg", null, new Exception(ex.Message));
                }
            }
            return result;
        }

        public override async Task< Result<WarehouseTransfer>> Add(WarehouseTransfer entity)
        {
            return await Task.Factory.StartNew<Result<WarehouseTransfer>>(() => {
                throw new NotImplementedException();
            });
        }

        public override async Task<Result<WarehouseTransfer>> Update(WarehouseTransfer entity)
        {
            return await Task.Factory.StartNew<Result<WarehouseTransfer>>(() => {
                throw new NotImplementedException();
            });
        }
    }
}
