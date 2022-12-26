using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WebERPV3.Entities;

namespace WebERPV3.Repository
{
    public class DataRepositoriesFactory : IDataRepositoryFactory
    {
        private readonly IServiceProvider services;

        public DataRepositoriesFactory() { }

        public DataRepositoriesFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IBase<T> GetDataRepositories<T>() where T : class, IBaseEntity, new()
        {
            //Import instance of T from the DI container
            var instance = services.GetService<IBase<T>>();

            return instance;
        }

        public TRepositories GetCustomDataRepositories<TRepositories>() where TRepositories : IBase
        {
            //Import instance of the Repositories from the DI container
            var instance = services.GetService<TRepositories>();

            return instance;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            //Import instance of T from the DI container
            var instance = services.GetService<IUnitOfWork>();

            return instance;
        }
    }
}
