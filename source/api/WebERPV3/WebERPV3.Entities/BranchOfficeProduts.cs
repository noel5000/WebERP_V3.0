using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebERPV3.Entities
{ 
    public class BranchOfficeProduts

    {

        public long BranchOfficeId { get; set; }
        [ForeignKey("BranchOfficeId")]
        public BranchOffice? BranchOffice { get; set; }
        public List<Inventory>? ProductsByWarehouse { get; set; }

    }
}
