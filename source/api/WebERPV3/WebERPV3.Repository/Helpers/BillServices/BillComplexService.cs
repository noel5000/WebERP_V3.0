using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public class BillComplexService : BillProductServiceBase
    {
        protected override async Task<Result<InvoiceDetail>> ProcessDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice lead)
        {
            var detailService = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceDetailRepository>();
            var productRepo = dataRepositoryFactory.GetDataRepositories<Product>();
            var productBasesRepo = dataRepositoryFactory.GetDataRepositories<CompositeProduct>();
            var productUnitRepo = dataRepositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            var productBases = detail.Product.BaseCompositeProducts == null || detail.Product.BaseCompositeProducts.Count() == 0 ? (await productBasesRepo.GetAll(x=>x.AsNoTracking().Where(y=>y.IsDeleted==true && y.ProductId==detail.ProductId))).Data.ToList() :
                detail.Product.BaseCompositeProducts.ToList();
            detail.WarehouseId = null;
            detail.UnitId = null;
            var detailResult = detailService.Add(detail);

            productBases.ForEach(async p =>
            {
                var currentProduct = ( productRepo.Get(p.BaseProductId)).Result.Data.FirstOrDefault();

                var currentUnit = p.UnitProductEquivalence == null ? p.BaseProductUnitId.HasValue ?
                productUnitRepo.Get(p.BaseProductUnitId.Value).Result.Data.FirstOrDefault() : null : p.UnitProductEquivalence;

                var instance = GetBillProductOrServiceInstance.GetBillingInstance(currentProduct);
                // decimal nuevaQuantity = p.Quantity * detail.Quantity;
                InvoiceDetail detailTemp = new InvoiceDetail(detail)
                {
                    Quantity = p.Quantity * detail.Quantity,
                    UnitId = currentUnit != null ? currentUnit.Unit.Id : p.BaseProductUnitId,
                    ProductId = p.BaseProductId,
                    ParentId = detailResult.Id,
                    Product = currentProduct,
                    Free = true
                };
                var result = instance.ProcessProductService(branchOfficeId, detailTemp, dataRepositoryFactory, lead).Result.Data.FirstOrDefault();
                if (result.SaveRegister)
                  await  detailService.Add(result);
            });
            detail.SaveRegister = false;
            return new Result<InvoiceDetail>(0,0,"ok_msg",new List<InvoiceDetail>() { detail});
        }

        protected override async Task<Result<InvoiceDetail>> ProcessReturnDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice lead)
        {
            var invoiceDetailRepo = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceDetailRepository>();
            var productRepo = dataRepositoryFactory.GetDataRepositories<Product>();
            var productBasesRepo = dataRepositoryFactory.GetDataRepositories<CompositeProduct>();
            var productUnitRepo = dataRepositoryFactory.GetDataRepositories<UnitProductEquivalence>();

            var baseDetails = (await invoiceDetailRepo.GetChildren(detail.Id)).ToList();
            baseDetails.ForEach(async p =>
            {
                var currentProduct = p.Product ?? productRepo.Get(p.ProductId).Result.Data.FirstOrDefault();



                var instance = GetBillProductOrServiceInstance.GetBillingInstance(currentProduct);
                // decimal nuevaQuantity = p.Quantity * detail.Quantity;
                InvoiceDetail detailTemp = new InvoiceDetail()
                {
                    Id = p.Id,
                    IsDeleted = p.IsDeleted,
                    WarehouseId = p.WarehouseId ?? null,
                    Quantity = p.Quantity * (detail.ReturnAmount / detail.Quantity),
                    ReturnAmount = p.Quantity * (detail.ReturnAmount / detail.Quantity),
                    ParentId = p.ParentId ?? null,
                    Defective = detail.Defective,
                    InvoiceId = p.InvoiceId,
                    Product = p.Product,
                    ProductId = p.ProductId,
                    Unit = p.Unit,
                    UnitId = p.UnitId


                };

                var resultado = await instance.ReturnProductService(branchOfficeId, detailTemp, dataRepositoryFactory,lead);

            });

            return new Result<InvoiceDetail>(0, 0, "ok_msg", new List<InvoiceDetail>() { detail });
        }
    }
}
