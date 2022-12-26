using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class CreditNoteRepository : Repository<CreditNote>, ICreditNoteRepository
    {
        public CreditNoteRepository(MainContext context) : base(context)
        {
        }

        public async Task<CreditNote> GetBySequence(string sequence)
        {
            return await _Context.CreditNotes.AsNoTracking().FirstOrDefaultAsync(x => x.IsDeleted == false && x.Sequence == sequence);
        }
    }
}
