﻿using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        readonly ISequenceManagerRepository _sequenceRepo;
        public ProductRepository(MainContext context, ISequenceManagerRepository sequence) : base(context)
        {
            this._sequenceRepo = sequence;
        }

        public async Task<IEnumerable<Product>> GetFilteredAndLimited(int pageZise, string fieldName, string fieldName2, string searchCharacters)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

    public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

public async Task<IEnumerable<Product>> GetProductsOnlyFilteredAndLimited(int pageZise, string name, string searchCharacters)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

public async Task<IEnumerable<Product>> GetProductsOnlyFilteredAndLimited(int pageZise, string fieldName1, string fieldName2, string searchCharacters)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

public async Task<IEnumerable<Product>> GetServicesOnlyFilteredAndLimited(int pageZise, string fieldName, string searchCharacters)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

public async Task<IEnumerable<Product>> GetServicesOnlyFilteredAndLimited(int pageZise, string fieldName, string fieldName2, string searchCharacters)
        {
            return await Task.Factory.StartNew<IEnumerable<Product>>(() => {
                throw new NotImplementedException();
            });
        }

        public override async Task< Result<Product>> Add(Product entity)
        {

            Result<Product> result = new Result<Product>(-1, -1, "error_msg");

            using (var transaction = await _Context.Database.BeginTransactionAsync())
            {
                try
                {
                    entity.TranslationData = TranslateUtility.SaveTranslation(entity, entity.TranslationData);
                    var costs = entity.SuppliersCosts?.ToList();
                    var units = entity.ProductUnits?.ToList();
                    var taxes = entity.Taxes?.ToList();
                    var bases = entity.BaseCompositeProducts?.ToList();
                    entity.BaseCompositeProducts = null;
                    entity.SuppliersCosts = null;
                    entity.ProductUnits = null;
                    entity.Taxes = null;
                    decimal tempCost = (entity.IsService ? bases.Sum(x => x.TotalCost) : costs.Sum(x => x.Cost) / costs.Count);
                    entity.Cost = entity.Cost > tempCost ? entity.Cost :tempCost;
                    decimal tempPrice = (entity.IsService ? bases.Sum(x => x.TotalPrice) : entity.Price);
                    entity.Price = entity.Price == 0 ? tempPrice : entity.Price;
                    string sequence = await _sequenceRepo.CreateSequence(Common.Enums.SequenceTypes.Products);
                    entity.Sequence = sequence;
                     _Context.Products.Add(entity);
                    await _Context.SaveChangesAsync();
                  await  SetChildren(entity, costs, units, taxes, bases);


                    await transaction.CommitAsync();
                    result = new Result<Product>(0, 0, "ok_msg", new List<Product>() { new Product() { Id = entity.Id } });
                }
                catch (Exception ex)
                {
                    result = new Result<Product>(-1, -1, $"error_msg", null, ex);
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }

        public override async Task<Result<Product>> Update(Product entity)
        {
            Result<Product> result = new Result<Product>(-1, -1, "error_msg");

            using (var transaction = await _Context.Database.BeginTransactionAsync()) 
            {
                try
                {
                    
                    var dbEntity = await _Context.Products.FindAsync(entity.Id);
                    _Context.Entry<Product>(dbEntity).State = EntityState.Detached;
                    var translation = dbEntity as IEntityTranslate;
                    if (translation != null)
                    {
                        entity.TranslationData = TranslateUtility.SaveTranslation(entity, translation.TranslationData);

                    }
                    var costs = entity.SuppliersCosts?.ToList();
                    var units = entity.ProductUnits?.ToList();
                    var taxes = entity.Taxes?.ToList();
                    var bases = entity.BaseCompositeProducts?.ToList();
                    entity.BaseCompositeProducts = null;
                    entity.SuppliersCosts = null;
                    entity.ProductUnits = null;
                    entity.Sequence =string.IsNullOrEmpty(entity.Sequence)? await _sequenceRepo.CreateSequence(Common.Enums.SequenceTypes.Products):entity.Sequence;
                    entity.Taxes = null;
                    decimal tempCost = (entity.IsService ? bases.Sum(x => x.TotalCost) : costs.Sum(x => x.Cost)/costs.Count);
                    entity.Cost = entity.Cost > tempCost ? entity.Cost : tempCost;
                    decimal tempPrice = (entity.IsService ? bases.Sum(x => x.TotalPrice) : entity.Price);
                    entity.Price = entity.Price == 0 ? tempPrice : entity.Price;
                    _Context.Products.Update(entity);
                    await _Context.SaveChangesAsync();
                  
               await     SetChildren(entity,costs,units,taxes,bases);


                    await transaction.CommitAsync();
                    result = new Result<Product>(0, 0, "ok_msg", new List<Product>() { new Product() {Id=entity.Id } });
                }
                catch (Exception ex)
                {
                   result = new Result<Product>(-1, -1, $"error_msg",null,ex);
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }


        private async Task SetChildren(Product product,List<ProductSupplierCost> costs,List<UnitProductEquivalence> units,List<ProductTax>taxes,List<CompositeProduct> bases) 
        {
            var previousCost = await _Context.ProductSupplierCosts.Where(x => x.ProductId == product.Id && x.IsDeleted == false).AsNoTracking().ToListAsync();
            var previousUnits = await _Context.UnitProductsEquivalences.Where(x => x.ProductId == product.Id && x.IsDeleted == false).AsNoTracking().ToListAsync();
            var previousTaxes = await _Context.ProductTaxes.Where(x => x.ProductId == product.Id && x.IsDeleted == false).AsNoTracking().ToListAsync();
            var previousBases = await _Context.CompositeProducts.Where(x => x.ProductId == product.Id && x.IsDeleted == false).AsNoTracking().ToListAsync();
            var productIsBase = await _Context.CompositeProducts.Where(x => x.BaseProductId == product.Id && x.IsDeleted == false).AsNoTracking().ToListAsync();
            costs.ForEach( c => {
                c.ProductId = product.Id;
                c.Supplier = null;
                c.Product = null;
                if (c.Id > 0 && !previousCost.Any(x => x.Id == c.Id))
                    c.IsDeleted = true;
                else
                    c.IsDeleted = false;

                if (c.Id == 0)
                   _Context.ProductSupplierCosts.Add(c);
                else
                    _Context.ProductSupplierCosts.Update(c);
                _Context.SaveChanges();
            });
            previousCost.ForEach(async c=> {
                if (!costs.Any(x => x.Id == c.Id)) 
                {
                    c.IsDeleted = true;
                    _Context.ProductSupplierCosts.Update(c);
                    _Context.SaveChanges();
                }
            });
           

            units.ForEach( c => {
               
                c.ProductId = product.Id;
                c.Product = null;
                c.Unit = null;
                c.CostPrice = product.Cost/c.Equivalence;
                c.SellingPrice = GetUnitSellingPrice(product, c.Equivalence);

                if (c.Id > 0 && !previousUnits.Any(x => x.Id == c.Id))
                    c.IsDeleted = true;
                else
                    c.IsDeleted = false;

                if (c.Id == 0)
                     _Context.UnitProductsEquivalences.Add(c);
                else
                    _Context.UnitProductsEquivalences.Update(c);
                 _Context.SaveChanges();
            });
            previousUnits.ForEach(  c => {
                if (!units.Any(x => x.Id == c.Id))
                {
                    c.IsDeleted = true;
                    _Context.UnitProductsEquivalences.Update(c);
                     _Context.SaveChanges();
                }
            });

            taxes.ForEach( c => {
               
                c.ProductId = product.Id;
                c.Product = null;
                c.Tax = null;

                if (c.Id > 0 && !previousTaxes.Any(x => x.Id == c.Id))
                    c.IsDeleted = true;
                else
                    c.IsDeleted = false;

                if (c.Id == 0)
                     _Context.ProductTaxes.Add(c);
                else
                    _Context.ProductTaxes.Update(c);
                 _Context.SaveChanges();
            });


            previousTaxes.ForEach( c => {
                if (!taxes.Any(x => x.Id == c.Id))
                {
                    c.IsDeleted = true;
                    _Context.ProductTaxes.Update(c);
                     _Context.SaveChanges();
                }
            });
            bases.ForEach(async c => {
               
                c.BaseProduct = c.BaseProduct != null && c.BaseProduct.Id > 0 ? c.BaseProduct : await _Context.Products.FindAsync(c.BaseProductId);
                _Context.Entry<Product>(c.BaseProduct).State = EntityState.Detached;
                c.BaseProductUnitId = c.BaseProductUnitId == 0 ? null : c.BaseProductUnitId;
                c.UnitProductEquivalence = c.UnitProductEquivalence != null && c.UnitProductEquivalence.Id > 0 ? c.UnitProductEquivalence :c.BaseProductUnitId.HasValue ? await _Context.UnitProductsEquivalences.FindAsync(c.BaseProductUnitId):null;
                if(c.UnitProductEquivalence!=null)
                _Context.Entry<UnitProductEquivalence>(c.UnitProductEquivalence).State = EntityState.Detached;
                c.ProductId = product.Id;
                c.IsDeleted = false;
                c.CurrencyId = product.CurrencyId;
                c.Currency = null;
                c.TotalCost =c.UnitProductEquivalence!=null? c.Quantity * c.UnitProductEquivalence.CostPrice:c.BaseProduct.Cost*c.Quantity;
                c.TotalPrice =c.UnitProductEquivalence!=null?(c.Quantity*c.UnitProductEquivalence.SellingPrice): c.Quantity * (new decimal[] { product.Price, product.Price2, product.Price3 }.OrderByDescending(x => x).FirstOrDefault());
                c.BaseProduct = null;
                c.Product = null;
                c.UnitProductEquivalence = null;

                if (c.Id > 0 && !previousBases.Any(x => x.Id == c.Id))
                    c.IsDeleted = true;
                else
                    c.IsDeleted = false;

                if (c.Id == 0)
                     _Context.CompositeProducts.Add(c);
                else
                    _Context.CompositeProducts.Update(c);
                 _Context.SaveChanges();
            });
            productIsBase.ForEach(  b => {
                var unit = b.BaseProductUnitId.HasValue ? units.FirstOrDefault(x => x.Id == b.BaseProductUnitId):null;
                if (b.BaseProductUnitId.HasValue && unit == null)
                    throw new Exception("thisIsABaseProduct_error");

                b.TotalCost = unit != null ? b.Quantity * unit.CostPrice : product.Cost * b.Quantity;
                b.TotalPrice = unit != null ? b.Quantity * unit.SellingPrice : b.Quantity * (new decimal[] { product.Price, product.Price2, product.Price3 }.OrderByDescending(x => x).FirstOrDefault());
                _Context.CompositeProducts.Update(b);
                 _Context.SaveChanges();


            });


            previousBases.ForEach( c => {
                if (!bases.Any(x => x.Id == c.Id))
                {
                    c.IsDeleted = true;
                    _Context.CompositeProducts.Update(c);
                    _Context.SaveChanges();
                }
            });
        }

        private decimal GetUnitSellingPrice(Product product, decimal equivalence) 
        {
            decimal result = 0;
            decimal[] prices = new decimal[] { product.Price, product.Price2, product.Price3 }.Where(x=>x>0).ToArray();
            result = (prices.Sum(x => x) / prices.Length) / equivalence;
            return result;
        }
    }
}
