using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class UserRole : BaseEntity
    {
        public string UserId { get; set; }
        public long RoleId { get; set; }

        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }



    }
}
