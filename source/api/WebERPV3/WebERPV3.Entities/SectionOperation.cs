using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class SectionOperation : BaseEntity
    {
        public long? SectionId { get; set; }
        public long OperationId { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
        [ForeignKey("OperationId")]
        public virtual Operation Operation { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }

    }
}
