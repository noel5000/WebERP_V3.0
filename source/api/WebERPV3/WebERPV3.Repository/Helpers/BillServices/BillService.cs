using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public class BillService : BillProductServiceBase
    {
        protected override async Task< Result<InvoiceDetail>> ProcessDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice invoice)
        {
            var newDetail = new InvoiceDetail(detail);
            newDetail.WarehouseId = null;
            newDetail.UnitId = null;

           return await Task.Factory.StartNew((arg) => {
            return new Result<InvoiceDetail>(0, 0, "ok_msg", new List<InvoiceDetail>() { arg as InvoiceDetail });
        },newDetail);

         
        }

        protected override async Task<Result<InvoiceDetail>> ProcessReturnDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice invoice)
        {

            return await Task.Factory.StartNew((arg) => {
                return new Result<InvoiceDetail>(0, 0, "ok_msg", new List<InvoiceDetail>() { arg as InvoiceDetail });
            }, detail);
        }
    }
}
