using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository.Helpers
{
 public   class InvoiceHelper
    {
        public static Result<Invoice> ApplyCreditNote(Invoice invoice, CreditNote creditNoteToApply, out CreditNote creditNote)
        {
            creditNote = creditNoteToApply;
            if (!string.IsNullOrEmpty(invoice.AppliedCreditNote))
            {

               
                if (creditNoteToApply == null)
                    return new Result<Invoice>(-1, -1, $"creditNoteNotExist_msg | {invoice.AppliedCreditNote}");

                if (creditNoteToApply.Applied)
                    return new Result<Invoice>(-1, -1, $"creditNoteApplied_msg | {invoice.AppliedCreditNote},{creditNoteToApply.CreatedBy}");

                if (creditNoteToApply.CurrencyId != invoice.CurrencyId)
                    return new Result<Invoice>(-1, -1, $"differentCurrency_msg | {invoice.AppliedCreditNote},{creditNoteToApply.CreatedBy}");

                if (creditNoteToApply.Amount > invoice.TotalAmount)
                    return new Result<Invoice>(-1, -1, "amountIsGreater_msg");

                creditNoteToApply.Applied = true;

                creditNoteToApply.UpdatedBy = invoice.UpdatedBy;
                creditNoteToApply.ModifiedDate = DateTime.Now;
                creditNoteToApply.AppliedInvoiceNumber = invoice.InvoiceNumber;
                creditNote = creditNoteToApply;
                invoice.AppliedCreditNote = creditNoteToApply.Sequence;
                invoice.AppliedCreditNoteAmount = creditNoteToApply.Amount;
                invoice.PaidAmount += creditNoteToApply.Amount;
                // Invoice.MontoAdeudado -= creditNoteToApply.Monto;
                //invoice.InvoiceLeads.ForEach(d =>
                //{
                //    decimal porcentajeNC = (d.TotalAmount / invoice.TotalAmount) / d.Quantity;
                //    d.CreditNoteAmount = porcentajeNC * creditNoteToApply.Amount;


                //});

                return new Result<Invoice>(0,0,"ok_msg",new List<Invoice>() { invoice});

            }

            return new Result<Invoice>(0, 0, "ok_msg", new List<Invoice>() { invoice }); ;
        }
        public static async Task<CustomerPayment> ApplyInvoicePayment(CustomerPayment pago, ICustomerPaymentRepository paymentRepository)
        {
            
            return await paymentRepository.ApplyInvoicePayment(pago);
        }
    }
}
