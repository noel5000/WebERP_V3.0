using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface IInventoryEntryRepository: IBase<InventoryEntry>
    {
        Task<Result<object>> RemoveEntries(string sequence);
        Task<Result<object>> AddInventoryList(List<InventoryEntry> entries, string reference, string details);
    }
}
