using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Common.Interfaces;

namespace WebERPV3.Context
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public TenantService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._HttpContextAccessor = httpContextAccessor;
        }
        public string Tenant => this.GetTenant();


        public string GetTenant()
        {
            StringValues tenantId = new StringValues();
            this._HttpContextAccessor?.HttpContext?.Request?.Headers?.TryGetValue("plan-id", out tenantId);
            if (string.IsNullOrEmpty(tenantId))
            {
                Log.Error("Api Key required");
                tenantId = "";
                // throw new Exception("Api Key required");
            }
            return tenantId.ToString().Split(" ").LastOrDefault() ?? tenantId;
        }

    }
}
