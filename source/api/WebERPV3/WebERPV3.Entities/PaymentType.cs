using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebERPV3.Entities
{
    public class PaymentType : BaseEntity
    {

        public short Code { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public virtual IEnumerable<Payment> InvoicesPayments { get; set; }

        public virtual IEnumerable<ExpensesPayment> ExpensesPayments { get; set; }
        public virtual IEnumerable<Expense> Expenses { get; set; }




    }
}
