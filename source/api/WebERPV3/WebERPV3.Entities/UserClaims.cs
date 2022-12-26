using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebERPV3.Entities
{
    public class UserClaims : BaseEntity
    {
        public string UserId { get; set; }
        [MaxLength(100)]
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }


        [NotMapped]
        [IgnoreDataMember]
        public override string TranslationData { get; set; }

    }
}
