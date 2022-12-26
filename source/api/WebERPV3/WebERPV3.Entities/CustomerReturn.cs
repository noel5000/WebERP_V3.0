using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class CustomerReturn : BaseEntity
    {

        public Invoice Invoice { get; set; }


        public List<CustomerReturnDetail> ReturnDetails { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }






    }
}
