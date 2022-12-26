using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ISupplierReturnRepository: IBase<SupplierReturn>
    {
        Task<Result<object>> AddInventoryList(List<SupplierReturn> entries, string reference, string details);
        Task<Result<object>> RemoveEntries(string sequence);
    }
}
