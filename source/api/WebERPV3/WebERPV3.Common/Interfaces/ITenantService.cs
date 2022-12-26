using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Common.Interfaces
{
    public interface ITenantService
    {
        string Tenant { get; }

        string GetTenant();
    }
}
