using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebERPV3.Repository
{
    public interface IDataRepositoryFactory
    {
        IBase<T> GetDataRepositories<T>() where T : class, IBaseEntity, new();
        TRepositories GetCustomDataRepositories<TRepositories>() where TRepositories : IBase;
        IUnitOfWork GetUnitOfWork();
    }
}
