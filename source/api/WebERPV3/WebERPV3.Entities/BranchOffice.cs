using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebERPV3.Entities
{
    public class BranchOffice : BaseEntity
    {
        [MaxLength(100)]
        [Translate]
        [Required]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }

        public virtual IEnumerable<Warehouse> Warehouses { get; set; }

        public virtual IEnumerable<User> Users { get; set; }
        public virtual IEnumerable<CashRegister> CashRegisters { get; set; }


    }
}
