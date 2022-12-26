using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebERPV3.Context;
using WebERPV3.Entities;

namespace WebERPV3.Repository
{
    public class Repository<T> : Base<T, MainContext> where T : class, IBaseEntity, new()
    {
        public Repository(MainContext context) : base(context) { }
    }
}
