using WebERPV3.Common;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ISequenceManagerRepository : IBase<SequenceControl>
    {
        Task<string> CreateSequence(Enums.SequenceTypes sequenceCode);
    }
}
