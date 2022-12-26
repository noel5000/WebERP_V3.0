
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebERPV3.Entities
{






    public class ExportAttribute : Attribute
    {

      public  int Order { get; set; } = 0;
        public string ChildProperty { get; set; } = "";
        public ExportAttribute() { }

        public ExportAttribute(int Order)
        {
            this.Order = Order;
        }
        public ExportAttribute(int Order,string ChildPropertyName)
        {
            this.Order = Order;
            this.ChildProperty = ChildPropertyName;
        }
    }


 
}
