using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebERPV3.Entities
{
    public class CashRegister : BaseEntity
    {


        public long BranchOfficeId { get; set; }
        [MaxLength(3)]
        [Required]
        public string Code { get; set; }
        [MaxLength(100)]
        [Translate]
        [Required]
        public string Name { get; set; }

        [ForeignKey("BranchOfficeId")]
        public BranchOffice BranchOffice { get; set; }

    }
}
