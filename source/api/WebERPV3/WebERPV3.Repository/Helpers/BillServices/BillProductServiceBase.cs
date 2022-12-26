using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public abstract class BillProductServiceBase : IBillProductsServices
    {
        public async Task<Result<InvoiceDetail>> ProcessProductService(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory _dataRepositoryFactory, Invoice lead)
        {
            return await ProcessDetail(branchOfficeId, detail, _dataRepositoryFactory, lead);
        }

        public async Task<Result<InvoiceDetail>> ReturnProductService(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory _dataRepositoryFactory, Invoice lead)
        {
            return await ProcessReturnDetail(branchOfficeId, detail, _dataRepositoryFactory, lead);
        }

        protected abstract  Task<Result<InvoiceDetail>> ProcessDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice lead);

        protected abstract Task<Result<InvoiceDetail>> ProcessReturnDetail(long branchOfficeId, InvoiceDetail detail, IDataRepositoryFactory dataRepositoryFactory, Invoice lead);

    }
}
