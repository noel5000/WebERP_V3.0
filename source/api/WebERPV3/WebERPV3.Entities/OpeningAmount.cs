using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class OpeningAmount : BaseEntity
    {

        public decimal Amount { get; set; }
        public long Order { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }

    }
}
