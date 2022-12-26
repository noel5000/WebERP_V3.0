using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public interface ICustomerPaymentRepository: IBase<CustomerPayment>
    {
        Task<CustomerPayment> ApplyInvoicePayment(CustomerPayment payment);
        public Task<object> IncomesReport(object searchParams);
        public Task<object> CommisionsReport(object searchParams);
    }
}
