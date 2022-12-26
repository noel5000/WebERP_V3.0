﻿using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IProductTaxRepository : IBase<ProductTax>
    {
        Task<IEnumerable<ProductTax>> GetProductTaxes(long productId);
    }
}
