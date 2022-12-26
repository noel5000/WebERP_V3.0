using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebERPV3.Entities
{
    public class MovementType : BaseEntity
    {


        public short Code { get; set; }

        [MaxLength(100)]
        [Translate]
        public string Name { get; set; }

       


    }
}
