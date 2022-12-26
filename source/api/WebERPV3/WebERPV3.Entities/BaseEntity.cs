using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Entities
{
    public abstract class BaseEntity: IBaseEntity
    {
        [MaxLength(50)]
        public virtual string PlanId { get; set; }
        [Key]
        public virtual long Id { get; set; }
        [MaxLength(50)]
        public virtual string? CreatedBy { get; set; }
        [MaxLength(50)]
        public virtual string? UpdatedBy { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        [MaxLength(1000)]
        public virtual  string? TranslationData { get; set; }
    }
}
