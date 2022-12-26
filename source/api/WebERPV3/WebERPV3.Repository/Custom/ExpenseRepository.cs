using Microsoft.EntityFrameworkCore;
using WebERPV3.Common;
using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebERPV3.Context;

namespace WebERPV3.Repository
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        readonly ISequenceManagerRepository _sequenceRepo;
        public ExpenseRepository(MainContext context, ISequenceManagerRepository sequenceManagerRepository) : base(context)
        {
            this._sequenceRepo = sequenceManagerRepository;
        }

        public async Task<IEnumerable<Expense>> GetDebsToPay(DateTime? startDate, DateTime? endDate, long? supplierId, long? currencyId, long? branchOfficeId)
        {
           
            return await _Context.Expenses.AsNoTracking().Where(
                expense => expense.IsDeleted == false && expense.OwedAmount > 0 && expense.State != (char)Enums.BillingStates.Nulled && (startDate.HasValue ? expense.Date >= startDate.Value : expense.Id > 0) &&
            (endDate.HasValue ? expense.Date <= endDate.Value : expense.Id > 0) && (supplierId.HasValue ? expense.SupplierId == supplierId.Value : expense.Id > 0) &&
            (currencyId.HasValue ? expense.CurrencyId == currencyId.Value : expense.Id > 0) && (branchOfficeId.HasValue ? expense.BranchOfficeId == branchOfficeId.Value : expense.Id > 0)
                ).ToListAsync();
        }

        public override async Task<Result<Expense>> Add(Expense entity)
        {
            Result<Expense> result = new Result<Expense>(-1, -1, "");

            using (var trans = await _Context.Database.BeginTransactionAsync())
            {
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                try
                {
                    entity.Sequence = await _sequenceRepo.CreateSequence(Enums.SequenceTypes.Expenses);
                    entity.ExpenseReference = entity.ExpenseReference ?? "";
                    var taxes = entity.Taxes?.ToList();
                    entity.Currency = entity.Currency ?? await _Context.Currencies.FindAsync(entity.CurrencyId);
                    _Context.Entry<Currency>(entity.Currency).State = EntityState.Detached;
                    var currency = entity.Currency;
                    taxes.ForEach(d =>
                    {
                        d.Id = 0;
                        d.Date = entity.Date;
                        d.CurrencyId = entity.CurrencyId;
                        d.Currency = entity.Currency;
                        d.Expense = null;
                        d.Reference = entity.Sequence;
                        d.Tax = d.Tax ?? _Context.Taxes.Find(d.TaxId);
                        _Context.Entry<Tax>(d.Tax).State = EntityState.Detached;
                        d.TaxAmount = d.Tax.Rate * entity.BeforeTaxesAmount;
                        d.ExchangeRateAmount = (d.Currency.ExchangeRate * d.TaxAmount);
                        d.Currency = null;
                        d.Tax = null;
                    });
                    entity.BranchOffice = null;
                    entity.PaymentType = null;
                    entity.TotalAmount = entity.BeforeTaxesAmount + taxes.Sum(x => x.TaxAmount);
                    entity.PaidAmount = entity.PaymentTypeId.HasValue ? (entity.GivenAmount >= entity.TotalAmount ? entity.TotalAmount : entity.GivenAmount) : 0;
                    entity.ReturnedAmount = entity.PaymentTypeId.HasValue ? (entity.GivenAmount > entity.TotalAmount ? entity.GivenAmount - entity.TotalAmount : 0) : 0;
                    entity.Supplier = null;
                    entity.Taxes = null;
                    entity.State = entity.PaidAmount > 0 ? (char)Enums.BillingStates.Paid : (char)Enums.BillingStates.Billed;
                    entity.OwedAmount = entity.TotalAmount - entity.PaidAmount;
                    entity.ExchangeRateAmount = entity.Currency.ExchangeRate * entity.TotalAmount;
                    entity.Currency = null;
                    _Context.Expenses.Add(entity);
                 await   _Context.SaveChangesAsync();

                    taxes.ForEach(x => { x.ExpenseId = entity.Id; });
                  await  _Context.ExpenseTaxes.AddRangeAsync(taxes);
                  await  _Context.SaveChangesAsync();
                    if (entity.PaymentTypeId.HasValue && entity.PaidAmount > 0)
                    {

                        var payment = new CompanyPayments()
                        {
                            PaidAmount = entity.PaidAmount,
                            TotalAmount = entity.TotalAmount,
                            GivenAmount = entity.GivenAmount,
                            PaymentDestinationId = entity.SupplierId,
                            Date = DateTime.Now,
                            CurrencyId = entity.CurrencyId,
                            ExchangeRate = currency.ExchangeRate,
                            OutstandingAmount = entity.OwedAmount,
                            ExchangeRateAmount = entity.PaidAmount * currency.ExchangeRate,
                            DestinationType = (byte)Enums.CompanyPaymentTypes.ExpensePayment,
                            Details = entity.Details,
                            Reference = entity.ExpenseReference,
                            Sequence =await _sequenceRepo.CreateSequence(Enums.SequenceTypes.CompanyPayments),
                            PaymentTypeId = entity.PaymentTypeId.Value,
                            State = entity.State,
                            PositiveBalance = entity.GivenAmount - entity.PaidAmount

                        };
                        _Context.CompanyPayments.Add(payment);
                       await _Context.SaveChangesAsync();
                        ExpensesPayment expensePayment = new ExpensesPayment()
                        {
                            Date = DateTime.Now,
                            Sequence = await _sequenceRepo.CreateSequence(Enums.SequenceTypes.ExpensePayments),
                            PaymentSequence = payment.Sequence,
                            PaidAmount = entity.PaidAmount,
                            TotalAmount = entity.TotalAmount,
                            State = (char)Enums.BillingStates.Paid,
                            CurrencyId = entity.CurrencyId,
                            Details = entity.Details,
                            ExchangeRate = currency.ExchangeRate,
                            ExchangeRateAmount = entity.PaidAmount * currency.ExchangeRate,
                            PositiveBalance = entity.GivenAmount - entity.PaidAmount,
                            GivenAmount = entity.GivenAmount,
                            PaymentId = payment.Id,
                            ExpenseCurrencyId = entity.CurrencyId,
                            PaymentType = null,
                            ExpenseId = entity.Id,
                            PaymentTypeId = entity.PaymentTypeId.Value,
                            ExpenseReference = entity.Sequence,
                            OutstandingAmount = entity.OwedAmount,
                            CurrentOutstandingAmount = entity.OwedAmount,
                            SupplierId = entity.SupplierId
                        };
                        _Context.ExpensesPayments.Add(expensePayment);
                       await _Context.SaveChangesAsync();
                    }

                   await trans.CommitAsync();
                    result = new Result<Expense>(entity.Id, 0, "ok_msg", new List<Expense>() { entity });
                }
                catch (Exception ex)
                {
                  await  trans.RollbackAsync();
                    result = new Result<Expense>(-1, -1, "error_msg", null, new Exception(ex.Message));
                }
            }

            return result;
        }

        public override async Task<Result<Expense>> Update(Expense entity)
        {
            Result<Expense> result = new Result<Expense>(-1, -1, "");

            using (var trans = await _Context.Database.BeginTransactionAsync())
            {
                var oldValue = await _Context.Expenses.FindAsync(entity.Id);
                _Context.Entry<Expense>(oldValue).State = EntityState.Detached;
                if (oldValue == null || oldValue.State != (char)Enums.BillingStates.Billed || await _Context.ExpensesPayments.AsNoTracking().CountAsync(x => x.IsDeleted == false && x.State == (char)Enums.BillingStates.Paid && x.ExpenseId == entity.Id) > 0)
                {
                   await trans.RollbackAsync();
                    return new Result<Expense>(-1, -1, "CannotUpdateExpense_msg");
                }
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                try
                {

                    entity.ExpenseReference = entity.ExpenseReference ?? "";
                    entity.Date = DateTime.MinValue == entity.Date ? currentDate : entity.Date;
                    var taxes = entity.Taxes?.ToList();
                    entity.Currency = entity.Currency ?? await _Context.Currencies.FindAsync(entity.CurrencyId);
                    _Context.Entry<Currency>(entity.Currency).State = EntityState.Detached;
                    var currency = entity.Currency;
                    var oldTaxes = await _Context.ExpenseTaxes.AsNoTracking().Where(x => x.IsDeleted == false && x.ExpenseId == entity.Id).ToListAsync();
                    //oldTaxes.ForEach(o => {
                    //    if (!taxes.Any(x => x.Id == o.Id)) 
                    //    {
                    //        _Context.ExpenseTaxes.Remove(o);
                    //        _Context.SaveChanges();
                    //    }
                    //});

                    _Context.ExpenseTaxes.RemoveRange(oldTaxes.Where(x => !taxes.Any(y => y.Id == x.Id)));
                  await  _Context.SaveChangesAsync();
                    taxes.ForEach( d =>
                    {
                        d.Date = entity.Date;
                        d.CurrencyId = entity.CurrencyId;
                        d.Currency = entity.Currency;
                        d.Expense = null;
                        d.Reference = entity.Sequence;
                        d.Tax = d.Tax ?? _Context.Taxes.Find(d.TaxId);
                        _Context.Entry<Tax>(d.Tax).State = EntityState.Detached;
                        d.TaxAmount = d.Tax.Rate * entity.BeforeTaxesAmount;
                        d.ExchangeRateAmount = (d.Currency.ExchangeRate * d.TaxAmount);
                        d.Currency = null;
                        d.Tax = null;
                        if (d.Id == 0)
                            _Context.ExpenseTaxes.Add(d);
                        else
                            _Context.ExpenseTaxes.Update(d);

                       _Context.SaveChanges();
                    });
                    entity.BranchOffice = null;
                    entity.PaymentType = null;
                    entity.TotalAmount = entity.BeforeTaxesAmount + taxes.Sum(x => x.TaxAmount);
                    entity.PaidAmount = entity.PaymentTypeId.HasValue ? (entity.GivenAmount >= entity.TotalAmount ? entity.TotalAmount : entity.GivenAmount) : 0;
                    entity.ReturnedAmount = entity.PaymentTypeId.HasValue ? (entity.GivenAmount > entity.TotalAmount ? entity.GivenAmount - entity.TotalAmount : 0) : 0;
                    entity.Supplier = null;
                    entity.Taxes = null;
                    entity.State = entity.PaidAmount > 0 ? (char)Enums.BillingStates.Paid : (char)Enums.BillingStates.Billed;
                    entity.OwedAmount = entity.TotalAmount - entity.PaidAmount;
                    entity.ExchangeRateAmount = entity.Currency.ExchangeRate * entity.TotalAmount;
                    entity.Currency = null;
                    _Context.Expenses.Update(entity);
                  await  _Context.SaveChangesAsync();
                    if (entity.PaymentTypeId.HasValue && entity.PaidAmount > 0)
                    {
                        var payment = new CompanyPayments()
                        {
                            PaidAmount = entity.PaidAmount,
                            TotalAmount = entity.TotalAmount,
                            GivenAmount = entity.GivenAmount,
                            PaymentDestinationId = entity.SupplierId,
                            Date = DateTime.Now,
                            CurrencyId = entity.CurrencyId,
                            ExchangeRate = currency.ExchangeRate,
                            OutstandingAmount = entity.OwedAmount,
                            ExchangeRateAmount = entity.PaidAmount * currency.ExchangeRate,
                            DestinationType = (byte)Enums.CompanyPaymentTypes.ExpensePayment,
                            Details = entity.Details,
                            Reference = entity.ExpenseReference,
                            Sequence = await _sequenceRepo.CreateSequence(Enums.SequenceTypes.CompanyPayments),
                            PaymentTypeId = entity.PaymentTypeId.Value,
                            State = entity.State,
                            PositiveBalance = entity.GivenAmount - entity.PaidAmount

                        };
                        _Context.CompanyPayments.Add(payment);
                       await _Context.SaveChangesAsync();
                        ExpensesPayment expensePayment = new ExpensesPayment()
                        {
                            PaidAmount = entity.PaidAmount,
                            Date = DateTime.Now,
                            Sequence = await _sequenceRepo.CreateSequence(Enums.SequenceTypes.ExpensePayments),
                            TotalAmount = entity.TotalAmount,
                            State = (char)Enums.BillingStates.Paid,
                            CurrencyId = entity.CurrencyId,
                            Details = entity.Details,
                            ExchangeRate = currency.ExchangeRate,
                            ExchangeRateAmount = entity.PaidAmount * currency.ExchangeRate,
                            PositiveBalance = entity.GivenAmount - entity.PaidAmount,
                            GivenAmount = entity.GivenAmount,
                            PaymentId = payment.Id,
                            ExpenseCurrencyId = entity.CurrencyId,
                            PaymentType = null,
                            ExpenseId = entity.Id,
                            PaymentTypeId = entity.PaymentTypeId.Value,
                            ExpenseReference = entity.Sequence,
                            OutstandingAmount = entity.OwedAmount,
                            PaymentSequence = payment.Sequence,
                            CurrentOutstandingAmount = entity.OwedAmount,
                            SupplierId = entity.SupplierId
                        };
                        _Context.ExpensesPayments.Add(expensePayment);
                       await _Context.SaveChangesAsync();
                    }

                   await trans.CommitAsync();
                    result = new Result<Expense>(entity.Id, 0, "ok_msg", new List<Expense>() { entity });
                }
                catch (Exception ex)
                {
                  await  trans.RollbackAsync();
                    result = new Result<Expense>(-1, -1, "error_msg", null, new Exception(ex.Message));
                }
            }

            return result;
        }

        public override async Task< Result<Expense>> Remove(long id)
        {
            Result<Expense> result = new Result<Expense>(-1, -1, "");

            using (var trans = await _Context.Database.BeginTransactionAsync())
            {
                var entity = await _Context.Expenses.FindAsync(id);
                _Context.Entry<Expense>(entity).State = EntityState.Detached;

                if (entity == null || entity.State != (char)Enums.BillingStates.Billed || _Context.ExpensesPayments.AsNoTracking().Count(x => x.IsDeleted == false && x.State != (char)Enums.BillingStates.Paid && x.ExpenseId == id) > 0)
                {
                    await trans.RollbackAsync();
                    return new Result<Expense>(-1, -1, "CannotDeleteExpense_msg");
                }

                try
                {
                    entity.State = (char)Enums.BillingStates.Nulled;
                    _Context.Expenses.Update(entity);
                  await  _Context.SaveChangesAsync();
                 await   trans.CommitAsync();
                    result = new Result<Expense>(entity.Id, 0, "ok_msg", new List<Expense>() { entity });
                }
                catch (Exception ex)
                {
                   await trans.RollbackAsync();
                    result = new Result<Expense>(-1, -1, "error_msg", null, new Exception(ex.Message));
                }
            }

            return result;
        }
    }
}
