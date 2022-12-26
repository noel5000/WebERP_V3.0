using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers
{
  public  class ProductsRepoHelper
    {

        public static async Task<bool> ExistProductInInvoices(int productId, IDataRepositoryFactory dataRepositoryFactory)
        {
            var detailsRepo = dataRepositoryFactory.GetCustomDataRepositories<IInvoiceDetailRepository>();
            if ((await detailsRepo.GetByProductId(productId)).ToList().Count > 0)
                return true;
            return false;
        }

        public static async Task<bool> IsProductInWarehouse(int productId, IDataRepositoryFactory dataRepositoryFactory)
        {
            var inventoryRepo = dataRepositoryFactory.GetCustomDataRepositories<IInventoryRepository>();
            if ((await inventoryRepo.GetProductInventory(productId)).Where(e => e.Quantity > 0).Count() > 0)
                return true;
            return false;
        }

        public static async Task<bool> IsBaseProduct(int productId, IDataRepositoryFactory dataRepositoryFactory)
        {
            var repo = dataRepositoryFactory.GetCustomDataRepositories<ICompositeProductRepository>();
            if ( (await repo.GetDerivedProducts(productId)).Count() > 0)
                return true;
            return false;
        }

        public static async Task<bool> AddUnits(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            var unitsRepo = dataRepositoryFactory.GetDataRepositories<UnitProductEquivalence>();
            foreach(var u in product.ProductUnits) 
            {
                u.CreatedBy = product.CreatedBy;
                u.CreatedDate = DateTime.Now;
                u.IsDeleted = false;
                u.ProductId = product.Id;
                await unitsRepo.Add(u);
            }
           

            return true;
        }

        public static async Task<bool> InsertTaxes(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            var taxesRepo = dataRepositoryFactory.GetDataRepositories<ProductTax>();

            foreach (var u in product.Taxes)
            {
                u.CreatedBy = product.CreatedBy;
                u.CreatedDate = DateTime.Now;
                u.IsDeleted = false;
                u.ProductId = product.Id;
                await taxesRepo.Add(u);
            }

            return true;
        }

        public static async Task<bool> AddBaseProducts(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            var baseProductsRepo = dataRepositoryFactory.GetDataRepositories<CompositeProduct>();
            if (product.IsService && product.IsCompositeProduct)
                foreach (var u in product.BaseCompositeProducts)
                {
                    u.CreatedBy = product.CreatedBy;
                    u.CreatedDate = DateTime.Now;
                    u.IsDeleted = false;
                    u.ProductId = product.Id;
                    await baseProductsRepo.Add(u);
                }
          

            return true;
        }

        public static async Task<bool> UpdateProductUnits(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            var unitsRepo = dataRepositoryFactory.GetCustomDataRepositories<IUnitProductEquivalenceRepository>();
            List<UnitProductEquivalence> productUnits = product.ProductUnits!=null?product.ProductUnits.ToList(): new List<UnitProductEquivalence>();
            var previousUnits =(await unitsRepo.GetProductUnits(product.Id)).ToList();

            if (product.IsService)
            {
                productUnits = new List<UnitProductEquivalence>();
                var deletedUnits = previousUnits.Except(productUnits).ToList();
                if (deletedUnits != null)
                    deletedUnits.ForEach(async e => {await unitsRepo.Remove(e.Id); });
            }

            if (!product.IsService)
            {
                if (productUnits.Count == 0)
                    throw new Exception("productNeedsUnits_msg");

                if (!productUnits.Exists(u => u.IsPrimary))
                    throw new Exception("productNeedsPrimaryUnit_msg");
            }
            if (!product.IsService && previousUnits != null)
            {
                productUnits.ForEach(async u =>
                {
                    if (u.Id == 0)
                    {
                        u.IsDeleted = false;
                        u.CreatedBy = product.CreatedBy;
                        u.CreatedDate = product.CreatedDate;
                      await  unitsRepo.Add(u);
                    }
                    else
                    {
                        var previuosEquivalenceUnits = previousUnits.Where(e => e.Id == u.Id).FirstOrDefault();
                        previuosEquivalenceUnits.Equivalence = u.Equivalence;
                        previuosEquivalenceUnits.IsPrimary = u.IsPrimary;
                        previuosEquivalenceUnits.CostPrice = u.CostPrice;
                        previuosEquivalenceUnits.SellingPrice = u.SellingPrice;
                        previuosEquivalenceUnits.UpdatedBy = product.UpdatedBy;
                        previuosEquivalenceUnits.ModifiedDate = product.ModifiedDate;
                      await  unitsRepo.Update(previuosEquivalenceUnits);
                    }


                });

                var deletedUnits = previousUnits.Except(productUnits).ToList();
                if (deletedUnits != null)
                    deletedUnits.ForEach(async e =>
                    {
                        try
                        {
                          await  unitsRepo.Remove(e.Id);
                        }
                        catch
                        {
                            throw new Exception("cannotEraseUnit_msg");
                        }

                    });

            }
            return true;
        }

        public static async Task<bool> UpdateProductTaxes(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            List<ProductTax> productTaxes = product.Taxes!=null?product.Taxes.ToList() : new List<ProductTax>();
            var taxesRepo = dataRepositoryFactory.GetCustomDataRepositories<IProductTaxRepository>();
            var previousTaxes = (await taxesRepo.GetProductTaxes(product.Id)).ToList();


            if (previousTaxes != null)
            {
                productTaxes.ForEach(async u =>
                {
                    if (u.Id == 0)
                    {
                        u.IsDeleted = false;
                        u.CreatedBy = product.CreatedBy;
                        u.CreatedDate = product.CreatedDate;
                        u.ProductId = product.Id;
                    await    taxesRepo.Add(u);
                    }
                    else
                    {
                        var previuosEquivalenceUnits = productTaxes.Where(e => e.Id == u.Id).FirstOrDefault();

                        previuosEquivalenceUnits.TaxId = u.TaxId;
                        previuosEquivalenceUnits.UpdatedBy = product.UpdatedBy;
                        previuosEquivalenceUnits.ModifiedDate = product.ModifiedDate;
                      await  taxesRepo.Update(previuosEquivalenceUnits);
                    }


                });

                var deletedUnits = previousTaxes.Except(productTaxes).ToList();
                if (deletedUnits != null)
                    deletedUnits.ForEach(async e =>
                    {
                        try
                        {
                      await      taxesRepo.Remove(e.Id);
                        }
                        catch 
                        {
                            throw new Exception("cannotDeleteTax_msg");
                        }

                    });

            }
            return true;
        }

        public static async Task<bool> UpdateProductBases(Product product, IDataRepositoryFactory dataRepositoryFactory)
        {
            var repo = dataRepositoryFactory.GetCustomDataRepositories<ICompositeProductRepository>();
            List<CompositeProduct> productBases = product.BaseCompositeProducts!=null?product.BaseCompositeProducts.ToList() : new List<CompositeProduct>();
            var previousBases = (await repo.GetProductBases(product.Id)).ToList() ?? new List<CompositeProduct>();

            if (!product.IsService)
            {
                productBases = new List<CompositeProduct>();
                var deletedBases = previousBases.Except(productBases).ToList();
                if (deletedBases != null)
                    deletedBases.ForEach(async e => { await repo.Remove(e.Id); });
            }


            if (product.IsService && product.IsCompositeProduct)
            {
                productBases.ForEach(async u =>
                {
                    if (u.Id == 0)
                    {
                        u.IsDeleted = false;
                        u.CreatedBy = product.CreatedBy;
                        u.CreatedDate = product.CreatedDate;
                      await  repo.Add(u);
                    }
                    else
                    {
                        var previousEquivalenceBases = previousBases.Where(e => e.Id == u.Id).FirstOrDefault();
                        if (previousEquivalenceBases != null)
                        {
                            previousEquivalenceBases.Quantity = u.Quantity;
                            previousEquivalenceBases.BaseProductId = u.BaseProductId;
                            previousEquivalenceBases.ProductId = u.ProductId;
                            previousEquivalenceBases.BaseProductUnitId = u.BaseProductUnitId ?? null;
                            previousEquivalenceBases.UpdatedBy = product.UpdatedBy ;
                            previousEquivalenceBases.ModifiedDate = product.ModifiedDate;
                         await   repo.Update(previousEquivalenceBases);
                        }

                    }


                });

                var deletedUnits = previousBases.Except(productBases).ToList();
                if (deletedUnits != null)
                    deletedUnits.ForEach(async e =>
                    {
                        try
                        {
                         await   repo.Remove(e.Id);
                        }
                        catch 
                        {
                            throw new Exception("cannotRemoveBaseProduct_msg");
                        }

                    });

            }
            else if ((product.IsService && !product.IsCompositeProduct) || !product.IsService)
            {
                previousBases.ForEach(async e =>
                {
                    try
                    {
                      await  repo.Remove(e.Id);
                    }
                    catch 
                    {
                        throw new Exception("cannotRemoveBaseProduct_msg");
                    }

                });


            }
            return true;
        }
    }
}
