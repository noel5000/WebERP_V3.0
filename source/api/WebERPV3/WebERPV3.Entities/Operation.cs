using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebERPV3.Entities
{
    public class Operation : BaseEntity
    {
        [MaxLength(100)]
        [Translate]
        public string Name { get; set; }

        public IEnumerable<SectionOperation> Sections { get; set; }

    }
}
