using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public class GetBillProductOrServiceInstance
    {
        private static IBillProductsServices GetBillingClass(Product product)
        {
            string clase = ClassList.Where(x => x.IsComposite == product.IsCompositeProduct && x.IsService == product.IsService).FirstOrDefault()?.ClassName;

            if (string.IsNullOrEmpty(clase))
                throw new Exception("notExistingClass_msg.");

            Type serviceType = Type.GetType(clase);
            return (IBillProductsServices)Activator.CreateInstance(serviceType);

        }

        public static IBillProductsServices GetBillingInstance(Product product)
        {
            IBillProductsServices instance = GetBillingClass(product);
            return instance;
        }

        private class ProductServiceBillingClass
        {
            public bool IsComposite { get; set; }
            public bool IsService { get; set; }
            public string ClassName { get; set; }
        }

        private static List<ProductServiceBillingClass> ClassList = new List<ProductServiceBillingClass>()
        {
        new ProductServiceBillingClass()
        {
        ClassName=$"WebERPV3.Repository.Helpers.BillServices.BillComplexService, WebERPV3.Repository, culture=neutral , version=1.0.0.0",
        IsComposite=true,
        IsService=true
        },
        new ProductServiceBillingClass()
        {
         ClassName=$"WebERPV3.Repository.Helpers.BillServices.BillService, WebERPV3.Repository, culture=neutral , version=1.0.0.0",
        IsComposite=false,
        IsService=true
        },
        new ProductServiceBillingClass()
        {
        ClassName=$"WebERPV3.Repository.Helpers.BillServices.BillProduct, WebERPV3.Repository, culture=neutral , version=1.0.0.0",
        IsComposite=false,
        IsService=false,
        }
        };
    }
}
