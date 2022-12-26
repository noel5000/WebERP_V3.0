using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class SellerRepository : Repository<Seller>, ISellerRepository
    {
        readonly ISequenceManagerRepository _sequenceRepo;
        public SellerRepository(MainContext context, ISequenceManagerRepository sequenceRepo) : base(context)
        {
            this._sequenceRepo = sequenceRepo;
        }

        public override async Task<Result<Seller>> Add(Seller entity)
        {
            entity.Code = await _sequenceRepo.CreateSequence(Common.Enums.SequenceTypes.Sellers);
            return await base.Add(entity);
        }

        public override async Task<Result<Seller>> Update(Seller entity)
        {
            entity.Code = string.IsNullOrEmpty(entity.Code) ? await _sequenceRepo.CreateSequence(Common.Enums.SequenceTypes.Sellers) : entity.Code;
            return await base.Update(entity);
        }

    }
}
