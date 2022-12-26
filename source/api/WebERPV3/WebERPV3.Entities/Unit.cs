using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebERPV3.Entities
{
    public class Unit : BaseEntity
    {
        [MaxLength(100)]
        [Translate]
        public string Name { get; set; }
    }
}
