using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebERPV3.Repository
{

    public interface IUnitOfWork
    {
        IDbContextTransaction CreateTransaction();
        Task<IDbContextTransaction> CreateTransactionAsync();
        Task<int> SaveChangesAsync();

        int SaveChanges();
        Task DisposeAsync();
    }
}
