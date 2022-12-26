using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using WebERPV3.Repository.Helpers.BillServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers
{
 public   class InventoryHelper
    {
        public static async Task< Result<object>> UpdateInventory(InvoiceDetail detail, Warehouse currentWarehouse, Invoice Invoice, IDataRepositoryFactory _repositoryFactory)
        {
            var warehouseMovementRepo = _repositoryFactory.GetDataRepositories<WarehouseMovement>();
            var productUnitsRepo = _repositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            var productRepo = _repositoryFactory.GetDataRepositories<Product>();
            var inventoryRepo = _repositoryFactory.GetDataRepositories<Inventory>();
            Inventory currentInventory = await inventoryRepo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.ProductId == detail.ProductId && y.WarehouseId == currentWarehouse.Id));
            if (currentInventory != null)
            {
                var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
       detail.Quantity,
       detail.UnitId.Value,
      detail.Product.ProductUnits.ToList()
       );
                if (convertionResult.Status >= 0)
                    detail.Quantity =(decimal) convertionResult.Data.FirstOrDefault();
                else
                    return convertionResult;

                detail.UnitId = detail.Product.ProductUnits.FirstOrDefault(pu => pu.IsPrimary).UnitId;
                convertionResult = ProductsHelper.ConvertFromProductPrincipalUnit(
       currentInventory.Quantity,
       detail.Product.ProductUnits.Where(pu => pu.IsPrimary).FirstOrDefault().UnitId,
      detail.Product.ProductUnits.ToList()
       );

                if (convertionResult.Status >= 0)
                    currentInventory.Quantity = (decimal)convertionResult.Data.FirstOrDefault();
                else
                    return convertionResult;


                if (currentInventory.Quantity < detail.Quantity)
                    return new Result<object>(-1, -1, $"outOfStock_msg | {detail.Product.Name},{currentWarehouse.Name}");
                else
                {
                    currentInventory.Quantity -= detail.Quantity;
                    Product product = (await productRepo.Get(detail.ProductId)).Data?.FirstOrDefault();
                    product.Existence -= detail.Quantity;
                   await productRepo.Update(product);
                   await inventoryRepo.Update(currentInventory);


                    var units = detail.Product.ProductUnits ?? (await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data;
                    WarehouseMovement movement = new WarehouseMovement(currentWarehouse.Id, detail.ProductId, detail.Quantity * -1,
                        true, units.Where(u => u.IsPrimary).FirstOrDefault().UnitId, 0, "OUT",
                        Invoice.InvoiceNumber ??Invoice.DocumentNumber?? string.Empty, currentInventory.Quantity);

                  await  warehouseMovementRepo.Add(movement);
                    return new Result<object>(0, 0, "ok_msg");


                }
            }
            else
                return new Result<object>(-1, -1, $"outOfStock_msg | {detail.Product.Name},{currentWarehouse.Name}");

        }


        public static async Task<Result<object>> AddInventory(InvoiceDetail detail, Invoice Invoice, IDataRepositoryFactory _repositoryFactory)
        {
            var warehouseMovementRepo = _repositoryFactory.GetDataRepositories<WarehouseMovement>();
            var productUnitsRepo = _repositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            var productRepo = _repositoryFactory.GetDataRepositories<Product>();
            var inventoryRepo = _repositoryFactory.GetDataRepositories<Inventory>();
            if (!detail.Product.IsService)
            {
                detail.Product.ProductUnits = detail.Product.ProductUnits ?? (await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data.ToList();
               
              
                if (detail.WarehouseId.HasValue)
                {
                    Inventory currentInventory = await inventoryRepo.Get(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId && y.WarehouseId==detail.WarehouseId.Value));
                    if (currentInventory != null)
                    {
                        var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
               currentInventory.Quantity,
               detail.Product.ProductUnits.Where(pu => pu.IsPrimary).FirstOrDefault().UnitId,
               detail.Product.ProductUnits.ToList()
               );
                        if (convertionResult.Status < 0)
                            return convertionResult;

                        currentInventory.Quantity = (decimal)convertionResult.Data.FirstOrDefault();

                        convertionResult= ProductsHelper.ConvertToProductPrincipalUnit(
               detail.Quantity,
               detail.UnitId.Value,
                detail.Product.ProductUnits.ToList()
               );
                        if (convertionResult.Status < 0)
                            return convertionResult;

                        detail.Quantity = (decimal)convertionResult.Data.FirstOrDefault();
                        var units = detail.Product.ProductUnits ??(await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data.ToList();
                       

                        currentInventory.Quantity += detail.Quantity;
                        Product Product =(await productRepo.Get(detail.ProductId)).Data.FirstOrDefault();
                        Product.Existence += detail.Quantity;
                      await  productRepo.Update(Product);
                      await  inventoryRepo.Update(currentInventory);

                        WarehouseMovement movimientoAlmacen = new WarehouseMovement(currentInventory.WarehouseId, detail.ProductId, detail.Quantity, true,
                           units.Where(u => u.IsPrimary).FirstOrDefault().UnitId, 0, "IN", Invoice.InvoiceNumber ?? Invoice.DocumentNumber ?? string.Empty, currentInventory.Quantity);
                      await  warehouseMovementRepo.Add(movimientoAlmacen);
                        return new Result<object>(0, 0, "ok_msg");
                    }
                    else
                        return new Result<object>(-1, -1, $"outOfStock_msg | {detail.Product.Name}");
                }
            }

            return new Result<object>(0, 0, "ok_msg");

        }


        public static async Task< Result<object>> ReInsertInventoryToWarehouse(InvoiceDetail detail, IDataRepositoryFactory _repositoryFactory, Warehouse currentWarehouse)
        {
            var productUnitsRepo = _repositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            var inventoryRepo = _repositoryFactory.GetDataRepositories<Inventory>();
            var warehouseMovementRepo = _repositoryFactory.GetDataRepositories<WarehouseMovement>();
            var productRepo = _repositoryFactory.GetDataRepositories<Product>();

            Inventory currentInventory =  await inventoryRepo.Get(x=>x.AsNoTracking().Where(y=>y.ProductId==detail.ProductId && y.WarehouseId== currentWarehouse.Id && y.IsDeleted==true));
            if (currentInventory != null)
            {
                detail.Product.ProductUnits = detail.Product.ProductUnits ?? (await productUnitsRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted && y.ProductId==detail.ProductId))).Data.ToList();

                var convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
       currentInventory.Quantity,
       detail.Product.ProductUnits.Where(pu => pu.IsPrimary).FirstOrDefault().UnitId,
        detail.Product.ProductUnits.ToList()
       );
                if (convertionResult.Status < 0)
                    return convertionResult;
                currentInventory.Quantity = (decimal)convertionResult.Data.FirstOrDefault();

                convertionResult = ProductsHelper.ConvertToProductPrincipalUnit(
           detail.Quantity,
           detail.UnitId.Value,
           detail.Product.ProductUnits.ToList()
           );
                if (convertionResult.Status < 0)
                    return convertionResult;

                detail.Quantity = (decimal)convertionResult.Data.FirstOrDefault();

                currentInventory.Quantity += detail.Quantity;
                Product Product = (await productRepo.Get(detail.ProductId)).Data.FirstOrDefault();
                Product.Existence += detail.Quantity;
              await  productRepo.Update(Product);
               await inventoryRepo.Update(currentInventory);

            }
            else
              await  inventoryRepo.Add(new Inventory()
                {
                    IsDeleted = false,
                    Quantity = detail.Quantity,
                    WarehouseId = currentWarehouse.Id,
                    CreatedBy = detail.CreatedBy,
                    CreatedDate = DateTime.Now,
                    ProductId = detail.ProductId,
                    UnitId = detail.Product.ProductUnits.Where(u => u.IsPrimary).FirstOrDefault().Id
                });

            return new Result<object>(0, 0, "ok_msg");

        }

        public static async Task< Result<InvoiceDetail>> UpdateProductInventory(BranchOffice branchOffice, InvoiceDetail detail, IDataRepositoryFactory _repositoryFactory, Invoice invoice)
        {
            InvoiceDetail newdetail = new InvoiceDetail(detail);
            var productRepo = _repositoryFactory.GetDataRepositories<Product>();
            var inventoryRepo = _repositoryFactory.GetDataRepositories<Inventory>();
            var detailRepo = _repositoryFactory.GetDataRepositories<InvoiceDetail>();

            var service = GetBillProductOrServiceInstance.GetBillingInstance(detail.Product);
            var result = await service.ProcessProductService(branchOffice.Id, detail, _repositoryFactory,invoice);
            if (result.Status >= 0)
                result.Data = new List<InvoiceDetail>() { newdetail };


            return result;
        }
    }
}
