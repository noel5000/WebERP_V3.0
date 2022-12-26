using Microsoft.EntityFrameworkCore;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebERPV3.Common.Enums;

namespace WebERPV3.Repository.Helpers
{
    public class SequencesHelper
    {
        public static async Task<string> CreateInvoiceControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.Invoices));

                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
               await repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        internal static async Task<string> CreateSellersControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.Sellers));

                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
               await repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }
        }

        public static async Task< string> CreateCustomersControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.Customers));

                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
               await repo.Update(lastControl);
                return result;
            }
            catch 
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        public static async Task<string> CreateQuotesControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.Quotes));

                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
              await  repo.Update(lastControl);
                return result;
            }
            catch 
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        public static async Task<string> CreateExpensesControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.Expenses));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
              await  repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        public static async Task<string> CreateWarehouseTransfersControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.WarehouseTransfers));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
             await   repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }



        }

        public static async Task<string> CreateInventoryIncomesControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.InventoryIncomes));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
             await   repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }



        }

        public static async Task<string> CreateCustomersReturnsControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.CustomersReturns));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
              await  repo.Update(lastControl);
                return result;
            }
            catch 
            {
                throw new Exception($"sequenceError_msg");
            }



        }

        public static  async Task<string>  CreateSupplierReturnsControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl =await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.SupplierReturns));

                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
                await repo.Update(lastControl);
                return result;
            }
            catch 
            {
                throw new Exception($"sequenceError_msg");
            }



        }

        public static async Task<string> CreatePaymentControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.CustomerPayments));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
                await repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        public static async Task<string> CreateExpensePaymentControl(IDataRepositoryFactory dataRepositoryFactory)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)SequenceTypes.ExpensePayments));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
                await repo.Update(lastControl);
                return result;
            }
            catch
            {
                throw new Exception($"sequenceError_msg");
            }


        }

        public static async Task<string> CreateSequenceControl(IDataRepositoryFactory dataRepositoryFactory, SequenceTypes sequenceTypes)
        {
            try
            {
                var repo = dataRepositoryFactory.GetDataRepositories<SequenceControl>();
                string result = string.Empty;
                SequenceControl lastControl = await repo.Get(x => x.AsNoTracking().Where(y => y.IsDeleted == false && y.Code == (short)sequenceTypes));
                result = String.Format("{0}{1:00000}", lastControl.Code, (lastControl.NumericControl + 1));
                lastControl.NumericControl += 1;
                await repo.Update(lastControl);
                return result;
            }
            catch 
            {
                throw new Exception($"sequenceError_msg");
            }


        }


    }
}
