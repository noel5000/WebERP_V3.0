using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebERPV3.Entities
{
    public class School : BaseEntity
    {

        [MaxLength(20)]
        [Export(Order = 2)]
        public string PhoneNumber { get; set; }

        public long CurrencyId { get; set; }
        public long CustomerId { get; set; }

        [MaxLength(200)]
        [Export(Order = 3)]
        public string Address { get; set; }
        [MaxLength(50)]
        [Export(Order = 1)]
        public string Code { get; set; }
        [Export(Order = 8)]
        public long LeadDueDays { get; set; }
        public long? AssignedWarehouseId { get; set; }
        public long? ZoneId { get; set; }


        [MaxLength(100)]
        [Export(Order = 0)]
        public string Name { get; set; }

        [NotMapped]
        public string NameAndCode { get { return $"{Code ?? ""}  {Name}"; } }



        [ForeignKey("CurrencyId")]
        [Export(Order = 4)]
        public Currency Currency { get; set; }
        [ForeignKey("ZoneId")]
        [Export(Order = 5)]
        public Zone Zone { get; set; }
        [ForeignKey("CustomerId")]
        [Export(Order = 6)]
        public Customer Customer { get; set; }

        [ForeignKey("AssignedWarehouseId")]
        [Export(Order = 7)]
        public Warehouse Warehouse { get; set; }

        public virtual IEnumerable<SchoolContact> Contacts { get; set; }
    }
}
