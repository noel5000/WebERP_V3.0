using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class BusinessStateRepository :  IBusinessStateRepository
    {
        private readonly MainContext _context;
        public BusinessStateRepository(MainContext context) 
        {
            this._context = context;
        }

        public async Task<List<object>> GetFinancialState(DateTime? startDate, DateTime? endDate)
        {
            return await Task.Factory.StartNew<List<object>>(() => {
                throw new NotImplementedException();
            });
        }
    }
}
