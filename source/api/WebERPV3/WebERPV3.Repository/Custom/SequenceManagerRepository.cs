using Microsoft.EntityFrameworkCore;
using WebERPV3.Common;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebERPV3.Common.Enums;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class SequenceManagerRepository : Repository<SequenceControl>, ISequenceManagerRepository
    {
        public SequenceManagerRepository(MainContext context) : base(context)
        {
        }

        public async Task<string> CreateSequence(Enums.SequenceTypes sequenceCode)
        {
            string result = "";
            var sequence = await _Context.SequencesControl.AsNoTracking().FirstOrDefaultAsync(x => x.IsDeleted == false && x.Code == (short)sequenceCode);
            
            if (sequence != null) 
            {
                result = String.Format("{0}{1:00000}", ((SequenceTypeCode)(short)sequence.Code).ToString() , (sequence.NumericControl + 1));
                sequence.NumericControl += 1;
                _Context.SequencesControl.Update(sequence);
             await   _Context.SaveChangesAsync();
            }
            return result;
        }

    }
}
