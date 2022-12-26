using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ICreditNoteRepository : IBase<CreditNote>
    {
        Task<CreditNote> GetBySequence(string sequence);
    }
}
