using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebERPV3.Common;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class CashRegisterOpening : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; }
        public long CurrencyId { get; set; }
        public long CashRegisterId { get; set; }
        public long BranchOfficeId { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime MaxClosureDate { get; set; }
        public DateTime? ClosureDate { get; set; }
        public decimal TotalPaymentsAmount { get; set; }
        public decimal OpeningClosureDifference { get; set; }

        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }
        public char State { get; set; }
        public decimal TotalOpeningAmount { get; set; }
        [NotMapped]
        public bool IsClosing { get; set; }
        public decimal TotalClosureAmount { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }

        [ForeignKey("CashRegisterId")]
        public CashRegister CashRegister { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

        [ForeignKey("BranchOfficeId")]
        public BranchOffice BranchOffice { get; set; }

        public virtual IEnumerable<CashRegisterOpeningDetail> Details { get; set; }

    }

    public class CashRegisterOpeningDetail : BaseEntity
    {
        public bool IsClosing { get; set; }
        public long CashRegisterOpeningId { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public decimal TotalAmount { get; set; }

        [ForeignKey("CashRegisterOpeningId")]
        public virtual CashRegisterOpening CashRegisterOpening { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }
    }
}
