using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public class BillProduct : BillProductServiceBase
    {
        protected override async Task<Result<InvoiceDetail>> ProcessDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice Invoice)
        {
            var inventoriesRepo = dataRepositoryFactory.GetDataRepositories<Inventory>();
            var warehouseRepo = dataRepositoryFactory.GetDataRepositories<Warehouse>();
            var productUnitsRepo = dataRepositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            InvoiceDetail newDetail = new InvoiceDetail(detail);
            detail.Product.ProductUnits = detail.Product.ProductUnits == null || detail.Product.ProductUnits.Count() == 0 ?
                    ( await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data.ToList() : detail.Product.ProductUnits;
            var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
newDetail.Quantity,
newDetail.UnitId.Value,
newDetail.Product.ProductUnits.ToList()
);

            if (convertionResult.Status < 0)
                return new Result<InvoiceDetail>(-1,-1,convertionResult.Message);

            decimal warehouseQuantity = (decimal)convertionResult.Data.FirstOrDefault();
            decimal quantitySumm = warehouseQuantity;

            InvoiceDetail newDetailCustom = new InvoiceDetail(newDetail)
            {

               Quantity = warehouseQuantity
            };
            Warehouse warehouse = null;
            if ((!Invoice.WarehouseId.HasValue  || Invoice.WarehouseId.Value<= 0))
            {
                var branchOfficeWarehouses = (await warehouseRepo.GetAll(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.BranchOfficeId == branchOfficeId
                && y.Code.ToLower() != "DEF"))).Data.ToList();
                // ojo ojo ojo
                var productInventory = await inventoriesRepo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == newDetail.ProductId &&
                y.Quantity >= warehouseQuantity && branchOfficeWarehouses.Select(r => r.Id).ToList().Contains(y.WarehouseId)));

                if (productInventory == null)
                    return new Result<InvoiceDetail>(-1, -1, "outOfStock_msg");
                // ojo ojo ojo

                warehouse = branchOfficeWarehouses.FirstOrDefault(x => x.Id == productInventory.WarehouseId);
            }
            else
            {
                warehouse = (await warehouseRepo.Get(Invoice.WarehouseId.Value)).Data.FirstOrDefault();
            }
          

            if ((Invoice.WarehouseId > 0) && (inventoriesRepo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == newDetail.ProductId && 
            y.Quantity >= warehouseQuantity && y.WarehouseId==Invoice.WarehouseId))==null))
                return new Result<InvoiceDetail>(-1, -1, $"outOfStock_msg | {detail.Product?.Name?? ""}");

            if (warehouse == null)
            {
                var branchOfficeWarehouses =( await warehouseRepo.GetAll(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.BranchOfficeId == branchOfficeId
                              && y.Code.ToLower() != "DEF"))).Data.ToList();
                // ojo ojo ojo
                var branchOfficeInventory = ( await inventoriesRepo.GetAll(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == newDetail.ProductId &&
                y.Quantity >0 && branchOfficeWarehouses.Select(r => r.Id).ToList().Contains(y.WarehouseId)))).Data.ToList();

                if (branchOfficeInventory == null)
                    return new Result<InvoiceDetail>(-1, -1, $"outOfStock_msg | {newDetail.Product.Name}");


                convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
 branchOfficeInventory.Sum(p => p.Quantity),
 branchOfficeInventory.FirstOrDefault().UnitId,
 newDetail.Product.ProductUnits.ToList()
 );
                if (convertionResult.Status < 0)
                    return new Result<InvoiceDetail>(-1, -1, convertionResult.Message);

                decimal warehousesQuantitySumm =(decimal) convertionResult.Data.FirstOrDefault() ;

                if (warehousesQuantitySumm < warehouseQuantity)
                    return new Result<InvoiceDetail>(-1, -1, $"outOfStock_msg | {newDetail.Product.Name}");

                foreach (Inventory warehouseProduct in branchOfficeInventory)
                {
                    if (quantitySumm <= 0)
                        break;
                    decimal currentWarehouseQuantity = warehouseProduct.Quantity;
                    if (quantitySumm < currentWarehouseQuantity)
                        currentWarehouseQuantity = quantitySumm;

                    newDetailCustom = new InvoiceDetail(newDetail);
                    newDetailCustom.Quantity =(decimal) ProductsHelper.ConvertFromProductPrincipalUnit(
 currentWarehouseQuantity,
 newDetail.UnitId.Value,
 newDetail.Product.ProductUnits.ToList()
 ).Data.FirstOrDefault();

                    newDetailCustom.WarehouseId = warehouseProduct.WarehouseId;
                    //  newDetailCustom.UnitId = newDetailCustom.Product.ProductUnits.FirstOrDefault(x => x.IsPrimary).UnitId;
               await     InventoryHelper.UpdateInventory(newDetailCustom,  ( await warehouseRepo.Get(warehouseProduct.WarehouseId)).Data.FirstOrDefault(), Invoice,dataRepositoryFactory);
                    newDetail.WarehouseId = warehouseProduct.WarehouseId;

                    quantitySumm -= currentWarehouseQuantity;
                }


            }
            else
            {
                newDetail.WarehouseId = warehouse.Id;
                newDetailCustom.Unit = newDetailCustom.Product.ProductUnits.FirstOrDefault(x => x.IsPrimary).Unit;
                newDetailCustom.UnitId = newDetailCustom.Unit.Id;
             await   InventoryHelper.UpdateInventory(newDetailCustom, warehouse, Invoice,dataRepositoryFactory);

            }

             return new Result<InvoiceDetail>(0, 0, "ok_msg", new List<InvoiceDetail>() { newDetail }) ;
        }

        protected override async Task<Result<InvoiceDetail>> ProcessReturnDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice invoice)
        {
            Inventory currentInventory = null;
            var warehouseRepo = dataRepositoryFactory.GetDataRepositories<Warehouse>();
            var productRepo = dataRepositoryFactory.GetDataRepositories<Product>();
           var inventoryRepo= dataRepositoryFactory.GetDataRepositories<Inventory>();
           var productUnitsRepo= dataRepositoryFactory.GetDataRepositories<UnitProductEquivalence>();
           var logMovementsRepo= dataRepositoryFactory.GetDataRepositories<WarehouseMovement>();

            if (detail.WarehouseId.HasValue)
            {
                detail.Product.ProductUnits = detail.Product.ProductUnits == null || detail.Product.ProductUnits.Count() == 0 ?
                   (await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data.ToList() : detail.Product.ProductUnits;

                if (detail.Defective)
                {
                    var defectiveWarehouse = await warehouseRepo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == "DEF" && y.BranchOfficeId == branchOfficeId));
                    if (defectiveWarehouse == null)
                        return new Result<InvoiceDetail>(-1, -1, $"defWarehouseNotExit_msg");

                    currentInventory =await inventoryRepo.Get(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.BranchOfficeId==branchOfficeId && y.ProductId==detail.ProductId &&
                    y.WarehouseId== defectiveWarehouse.Id)) ??
                new Inventory()
                {
                    WarehouseId = defectiveWarehouse.Id,
                    ProductId = detail.ProductId,
                    IsDeleted = false,
                    Quantity = 0,
                    CreatedBy = detail.CreatedBy,
                    CreatedDate = DateTime.Now,
                    BranchOfficeId = branchOfficeId,
                    UnitId = detail.Product.ProductUnits.FirstOrDefault(u => u.IsPrimary).UnitId,
                    Product = detail.Product
                };
                }
                else
                {
                    currentInventory = await inventoryRepo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == detail.ProductId && y.WarehouseId == detail.WarehouseId));
                   
                }
                if (currentInventory != null)
                {
                    currentInventory.Product.ProductUnits = currentInventory.Product.ProductUnits == null || currentInventory.Product.ProductUnits.Count() == 0 ?
                    (await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==currentInventory.ProductId))).Data.ToList(): currentInventory.Product.ProductUnits;

                    currentInventory.Quantity =(decimal) ProductsHelper.ConvertToProductPrincipalUnit(
         currentInventory.Quantity,
         detail.Product.ProductUnits.FirstOrDefault(pu => pu.IsPrimary).UnitId,
         detail.Product.ProductUnits.ToList()
         ).Data.FirstOrDefault();
                    detail.Quantity =(decimal) ProductsHelper.ConvertToProductPrincipalUnit(
           detail.Quantity,
           detail.UnitId.Value,
           detail.Product.ProductUnits.ToList()
           ).Data.FirstOrDefault();
                    var units = detail.Product.ProductUnits == null || detail.Product.ProductUnits.Count() == 0 ?
                   (await productUnitsRepo.GetAll(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == detail.ProductId))).Data.ToList() : detail.Product.ProductUnits;

                    var warehouseMov = new WarehouseMovement(currentInventory.WarehouseId, detail.ProductId, detail.Quantity,true, 
                        units.Where(u => u.IsPrimary).FirstOrDefault().UnitId, branchOfficeId, "IN", invoice.InvoiceNumber??invoice.DocumentNumber ?? string.Empty);
                  await  logMovementsRepo.Add(warehouseMov);

                    currentInventory.Quantity += detail.Quantity;
                    currentInventory.UnitId = currentInventory.Product.ProductUnits.FirstOrDefault(x => x.IsPrimary).UnitId;
                    Product Product = (await productRepo.Get(detail.ProductId)).Data.FirstOrDefault();
                    Product.Existence += detail.Quantity;
                   await productRepo.Update(Product);
                    if (currentInventory.Id > 0)
                  await     inventoryRepo.Update(currentInventory);
                    else
                   await     inventoryRepo.Add(currentInventory);
                }
                else
                    return new Result<InvoiceDetail>(-1,-1, "outOfStock_msg");

            }

            return new Result<InvoiceDetail>(0, 0, "ok_msg", new List<InvoiceDetail>() { detail });
        }
    }
}
