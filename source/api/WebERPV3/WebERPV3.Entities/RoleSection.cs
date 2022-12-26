using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WebERPV3.Entities
{
    public class RoleSection : BaseEntity
    {
        public long RoleId { get; set; }
        [NotMapped]
        public override string TranslationData { get; set; }
       public long SectionId { get; set; }
        [ForeignKey("SectionId")]
      public virtual Section Section { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

    }
}
