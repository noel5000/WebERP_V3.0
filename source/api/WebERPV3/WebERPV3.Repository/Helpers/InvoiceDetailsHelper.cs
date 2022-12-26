using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers
{
  public  class InvoiceDetailsHelper
    {
        public static async Task AddDetails(Invoice Invoice,  IDataRepositoryFactory dataRepositoryFactory, bool updateTaxes = true)
        {

            var invoiceDetailRepo = dataRepositoryFactory.GetDataRepositories<InvoiceDetail>();
            Invoice.InvoiceDetails.ForEach(async x =>
            {
                x.InvoiceId = Invoice.Id;
                x.CreatedDate = DateTime.Now;
                x.CreatedBy = Invoice.CreatedBy;
                x.IsDeleted = false;
                x.Date = DateTime.Now;
                x.ModifiedDate = DateTime.Now;
                var result = await InventoryHelper.UpdateProductInventory(Invoice.BranchOffice, x, dataRepositoryFactory, Invoice);
               
                if (result.Status < 0)
                    throw new Exception(result.Message);
                else
                    x = result.Data.FirstOrDefault();

                if (x.SaveRegister)
                   await invoiceDetailRepo.Add(x);
            });
            if (updateTaxes)
              await  UpdateInvoiceTaxes(Invoice, dataRepositoryFactory);

        }

        public static async Task< Result<Invoice>> UpdateDetails(Invoice invoice, IDataRepositoryFactory dataRepositoryFactory) 
        {
            var detailsRepo = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceDetailRepository>();
            var productUnitsRepo = dataRepositoryFactory.GetCustomDataRepositories<IUnitProductEquivalenceRepository>();
            var oldDetails = (await detailsRepo.GetByInvoiceId(invoice.Id)).ToList();
            var newDetails = invoice.InvoiceDetails.Where(x => x.Id == 0).ToList();
            var modifiedDetails = invoice.InvoiceDetails.Except(newDetails).ToList().Except(oldDetails).ToList();
            var untouchedDetails = invoice.InvoiceDetails.Intersect(oldDetails).ToList();


            
            oldDetails.ForEach( async d =>
            {
                bool existe = (modifiedDetails.Exists(x => x.ProductId == d.ProductId && d.ParentId == null) || untouchedDetails.Exists(x => x.ProductId == d.ProductId && d.ParentId == null));
                if (!existe)
                {
                   await detailsRepo.Remove(d.Id);
                 var result = await  InventoryHelper.AddInventory(d, invoice,dataRepositoryFactory);
                    if (result.Status < 0)
                        throw new Exception(result.Message);
                }
            });

       
            Invoice newinvoice = new Invoice(invoice);
            newinvoice.InvoiceDetails = newDetails;
         await   AddDetails(newinvoice, dataRepositoryFactory);


            modifiedDetails.ForEach(async d =>
            {
                if (!d.Product.IsService)
                {
                    var oldDetail = oldDetails.Where(x => x.Id == d.Id).FirstOrDefault();
                    decimal currentQuantity =(decimal) ProductsHelper.ConvertToProductPrincipalUnit(
           d.Quantity,
           d.UnitId.Value,
           d.Product.ProductUnits == null || d.Product.ProductUnits.Count() == 0 ?( await productUnitsRepo.GetProductUnits(d.ProductId)).ToList() : d.Product.ProductUnits.ToList()
           ).Data.FirstOrDefault();

                    decimal oldQuantity = (decimal)ProductsHelper.ConvertToProductPrincipalUnit(
            oldDetail.Quantity,
            oldDetail.UnitId.Value,
           oldDetail.Product.ProductUnits == null || oldDetail.Product.ProductUnits.Count() == 0 ? (await productUnitsRepo.GetProductUnits(oldDetail.ProductId)).ToList() : oldDetail.Product.ProductUnits.ToList()
            ).Data.FirstOrDefault();
                    decimal difference = currentQuantity - oldQuantity;
                    if (difference > 0)
                    {

                        var warehouse = invoice.BranchOffice.Warehouses?.Where(y => y.Id == d.WarehouseId).FirstOrDefault();
                        if (warehouse == null)
                            throw new Exception("warehouseError_msg");


                        InvoiceDetail nuevoDetalle = new InvoiceDetail()
                        {
                            Product = d.Product,
                            ProductId = d.ProductId,
                            Quantity = difference,
                            UnitId = d.UnitId ?? null,
                            Unit = d.Unit ?? null
                        };

                 var result=     await  InventoryHelper.UpdateInventory(nuevoDetalle, warehouse, invoice, dataRepositoryFactory);
                        if (result.Status < 0)
                            throw new Exception(result.Message);

                    }
                    else if (difference < 0)
                    {
                        InvoiceDetail detailToInsert = new InvoiceDetail(d);
                        detailToInsert.Quantity = Math.Abs(difference);
                        detailToInsert.Unit = d.Product.ProductUnits.Where(u => u.IsPrimary).FirstOrDefault().Unit;
                        detailToInsert.UnitId = detailToInsert.Unit.Id;
                        var result = await InventoryHelper.AddInventory(detailToInsert, invoice,dataRepositoryFactory);
                        if (result.Status < 0)
                            throw new Exception(result.Message);
                    }
                   await detailsRepo.Update(d);
                }
                else
                {
                 await   detailsRepo.Update(d);
                }

            });

         await   UpdateInvoiceTaxes(invoice, dataRepositoryFactory);
            return new Result<Invoice>(0,0,"ok_msg",new List<Invoice>() { invoice});
        }
       
        static async Task UpdateInvoiceTaxes(Invoice invoice, IDataRepositoryFactory dataRepositoryFactory)
        {
            var invoiceTaxRepo = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceTaxRepository>();
            var productTaxRepository = dataRepositoryFactory.GetCustomDataRepositories<IProductTaxRepository>();
            var invoiceTaxes = (await invoiceTaxRepo.GetInvoiceTaxes(invoice.InvoiceNumber)).ToList();
            if (invoiceTaxes != null && invoiceTaxes.Count > 0)
            {
                foreach (var tax in invoiceTaxes)
                {
                  await  invoiceTaxRepo.Remove(tax.Id);
                }
            }
            Dictionary<long, decimal> amountPerTax = new Dictionary<long, decimal>();
            invoice.InvoiceDetails.ForEach(async x =>
            {
                x.InvoiceId = invoice.Id;

                var productTaxes = x.Product?.Taxes != null && x.Product?.Taxes.Count() > 0 ? x.Product.Taxes :
               (await productTaxRepository.GetProductTaxes(x.ProductId));
                foreach (var impuesto in productTaxes)
                {
                    if (amountPerTax.Any(i => i.Key == impuesto.TaxId))
                    {
                        amountPerTax[impuesto.TaxId] += impuesto.Tax.Rate * x.BeforeTaxesAmount;
                    }
                    else
                        amountPerTax.Add(impuesto.TaxId, impuesto.Tax.Rate * x.BeforeTaxesAmount);
                }
            });

            foreach (var invoiceTaxAmount in amountPerTax)
            {
                InvoiceTax InvoiceImpuesto = new InvoiceTax()
                {
                    CreatedBy = invoice.CreatedBy,
                    IsDeleted = false,
                    InvoiceId = invoice.Id,
                    CreatedDate = DateTime.Now,
                    TaxId = invoiceTaxAmount.Key,
                    CurrencyId = invoice.CurrencyId,
                    TaxAmount = invoiceTaxAmount.Value,
                    InvoiceNumber = string.IsNullOrEmpty(invoice.InvoiceNumber) ? invoice.DocumentNumber : invoice.InvoiceNumber,

                };
               await invoiceTaxRepo.Add(InvoiceImpuesto);
            }
        }

        public static async Task< Result<Invoice>> UpdateQuoteDetails(Invoice invoice, IDataRepositoryFactory dataRepositoryFactory)
        {
            var detailsRepo = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceDetailRepository>();
            var oldDetails = (await detailsRepo.GetByInvoiceId(invoice.Id)).ToList();
            var newDetails = invoice.InvoiceDetails.Where(x => x.Id == 0).ToList();
            var modifiedDetails = invoice.InvoiceDetails.Except(newDetails).ToList().Except(oldDetails).ToList();
            var untouchedDetails = invoice.InvoiceDetails.Intersect(oldDetails).ToList();


            // BORRA LOS QUE YA NO EXISTEN EN LA invoice NUEVA
            oldDetails.ForEach(async d =>
            {
                bool existe = (modifiedDetails.Exists(x => x.ProductId == d.ProductId && d.ParentId == null) || untouchedDetails.Exists(x => x.ProductId == d.ProductId && d.ParentId == null));
                if (!existe)
                {
                  await  detailsRepo.Remove(d.Id);

                }
            });

            //INSERTAR DETALLES NUEVOS
            Invoice newinvoice = new Invoice(invoice);
            newinvoice.InvoiceDetails = newDetails;
         await   AddQuoteDetails(newinvoice,dataRepositoryFactory);


            modifiedDetails.ForEach(async d =>
            {

                await detailsRepo.Update(d);


            });

          await  UpdateInvoiceTaxes(invoice, dataRepositoryFactory);
            return new Result<Invoice>(0,0,"ok_msg",new List<Invoice>() { invoice});
        }

        public static async Task AddQuoteDetails(Invoice invoice, IDataRepositoryFactory dataRepositoryFactory)
        {
            var detailsRepo = dataRepositoryFactory.GetDataRepositories<InvoiceDetail>();

            invoice.InvoiceDetails.ForEach( async x =>
            {
                x.InvoiceId = invoice.Id;
                x.CreatedDate = DateTime.Now;
                x.CreatedBy = invoice.CreatedBy;
                x.IsDeleted = false;
                x.Date = DateTime.Now;
                x.ModifiedDate = DateTime.Now;
              await  detailsRepo.Add(x);

            });

         await   UpdateInvoiceTaxes(invoice, dataRepositoryFactory);
        }



    }
}
