using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebERPV3.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MainContext _DataContext;

        public UnitOfWork(MainContext dataContext)
        {
            _DataContext = dataContext;
        }

        public IDbContextTransaction CreateTransaction()
        {
            return this._DataContext.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> CreateTransactionAsync()
        {
            return await this._DataContext.Database.BeginTransactionAsync();
        }

        public int SaveChanges()
        {
            return _DataContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _DataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_DataContext != null)
            {
                _DataContext.Dispose();
            }
        }

        public async Task DisposeAsync()
        {
            if (_DataContext != null)
            {
              await  _DataContext.DisposeAsync();
            }
        }


    }
}
