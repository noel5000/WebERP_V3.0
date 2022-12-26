using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers.BillServices
{
    public interface IBillProductsServices
    {
        Task<Result<InvoiceDetail>> ProcessProductService(long branchOfficeId, InvoiceDetail details, IDataRepositoryFactory _dataRepositoryFactory, Invoice invoice);

        Task<Result<InvoiceDetail>> ReturnProductService(long branchOfficeId, InvoiceDetail details, IDataRepositoryFactory _dataRepositoryFactory, Invoice invoice);
    }
}
