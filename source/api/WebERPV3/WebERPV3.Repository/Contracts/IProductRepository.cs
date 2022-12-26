using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IProductRepository : IBase<Product>
    {
        Task<IEnumerable<Product>> GetProductsOnlyFilteredAndLimited(int pageZise, string name, string searchCharacters);
        Task<IEnumerable<Product>> GetServicesOnlyFilteredAndLimited(int pageZise, string fieldName, string searchCharacters);
        Task<IEnumerable<Product>> GetProductsOnlyFilteredAndLimited(int pageZise, string fieldName1, string fieldName2, string searchCharacters);
        Task<IEnumerable<Product>> GetServicesOnlyFilteredAndLimited(int pageZise, string fieldName, string fieldName2, string searchCharacters);
        Task<IEnumerable<Product>> GetFilteredAndLimited(int pageZise, string fieldName, string fieldName2, string searchCharacters);

        Task<IEnumerable<Product>> GetProductByName(string name);
    }
}
