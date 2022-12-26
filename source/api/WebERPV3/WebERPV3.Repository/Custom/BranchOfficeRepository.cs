using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class BranchOfficeRepository : Repository<BranchOffice>, IBranchOfficeRepository
    {
        
        public BranchOfficeRepository(MainContext context) : base(context)
        {
        }

      
    }
}
