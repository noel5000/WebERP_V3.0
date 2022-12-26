using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebERPV3.Common.Interfaces;
using WebERPV3.Entities;

namespace WebERPV3.Context
{
    public class MainContext : IdentityDbContext<User, Role, string>
    {
        private readonly ITenantService _TenantService;
        public MainContext(DbContextOptions<MainContext> options, ITenantService tenantService)
  : base(options)
        {
            _TenantService = tenantService;
        }
        #region Tables

        public virtual DbSet<CashRegister> CashRegisters { get; set; }
        public virtual DbSet<LanguageKey> LanguageKeys { get; set; }
        public virtual DbSet<CashRegisterOpening> CashRegisterOpenings { get; set; }
        public virtual DbSet<CashRegisterOpeningDetail> CashRegisterOpeningDetails { get; set; }
        public virtual DbSet<CustomerPayment> CustomersPayments { get; set; }
        public virtual DbSet<CompositeProduct> CompositeProducts { get; set; }
        public virtual DbSet<CreditNote> CreditNotes { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CompanyPayments> CompanyPayments { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<SchoolContact> SchoolContacts { get; set; }
        public virtual DbSet<CustomerBalance> CustomersBalance { get; set; }
        public virtual DbSet<CustomerReturn> CustomersReturns { get; set; }
        public virtual DbSet<CustomerReturnDetail> CustomersReturnDetails { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseTax> ExpenseTaxes { get; set; }
        public virtual DbSet<ExpensesPayment> ExpensesPayments { get; set; }
        public virtual DbSet<InventoryEntry> InventoryEntries { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceLead> InvoicesLeads { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<LeadDetail> LeadsDetails { get; set; }
        public virtual DbSet<InvoiceTax> InvoicesTaxes { get; set; }
        public virtual DbSet<BranchOffice> BranchOffices { get; set; }
        public virtual DbSet<MovementType> MovementTypes { get; set; }
        public virtual DbSet<OpeningAmount> OpeningsAmounts { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductTax> ProductTaxes { get; set; }
        public virtual DbSet<ReturnDetail> ReturnDetails { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<SequenceControl> SequencesControl { get; set; }
        public virtual DbSet<SupplierReturn> SuppliersReturns { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<SupplierBalance> SuppliersBalances { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<TRNControl> TRNsControl { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<UnitProductEquivalence> UnitProductsEquivalences { get; set; }
        public virtual DbSet<ProductSupplierCost> ProductSupplierCosts { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<WarehouseMovement> WarehousesMovements { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<WarehouseTransfer> WarehousesTransfers { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IBaseEntity>().HasQueryFilter(x => !string.IsNullOrEmpty(_TenantService.Tenant) && x.PlanId == _TenantService.Tenant);

            foreach (var property in builder.Model.GetEntityTypes()
          .SelectMany(t => t.GetProperties())
          .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetColumnType("decimal(18, 2)");
            }

            var cascadeFKs = builder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys())
                            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Get the entries that are auditable
            var auditableEntitySet = ChangeTracker.Entries<IBaseEntity>();

            if (auditableEntitySet != null)
            {

                DateTime currentDate = DateTime.Now;

                // Audit set the audit information foreach record
                foreach (var auditableEntity in auditableEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted))
                {

                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.IsDeleted = false;
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                        auditableEntity.Entity.CreatedDate = currentDate;
                    }

                    auditableEntity.Entity.ModifiedDate = currentDate;

                    if (auditableEntity.State == EntityState.Deleted)
                    {
                        auditableEntity.Entity.IsDeleted = true;
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                        auditableEntity.State = EntityState.Modified;
                    }

                    if (auditableEntity.State == EntityState.Modified)
                    {
                        auditableEntity.Property(nameof(IBaseEntity.CreatedDate)).IsModified = false;
                        auditableEntity.Property(nameof(IBaseEntity.PlanId)).IsModified = false;
                        auditableEntity.Property(nameof(IBaseEntity.CreatedBy)).IsModified = false;
                    }
                }
            }
            var tenantEntitySet = ChangeTracker.Entries<ITenant>();
            if (tenantEntitySet != null)
            {


                // Audit set the audit information foreach record
                foreach (var auditableEntity in tenantEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted))
                {

                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                    }

                    if (auditableEntity.State == EntityState.Deleted)
                    {
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                    }

                    if (auditableEntity.State == EntityState.Modified)
                    {
                        auditableEntity.Property(nameof(ITenant.PlanId)).IsModified = false;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            // Get the entries that are auditable
            var auditableEntitySet = ChangeTracker.Entries<IBaseEntity>();

            if (auditableEntitySet != null)
            {

                DateTime currentDate = DateTime.Now;

                // Audit set the audit information foreach record
                foreach (var auditableEntity in auditableEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted))
                {

                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.IsDeleted = false;
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                        auditableEntity.Entity.CreatedDate = currentDate;
                    }

                    auditableEntity.Entity.ModifiedDate = currentDate;

                    if (auditableEntity.State == EntityState.Deleted)
                    {
                        auditableEntity.Entity.IsDeleted = true;
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                        auditableEntity.State = EntityState.Modified;
                    }

                    if (auditableEntity.State == EntityState.Modified)
                    {
                        auditableEntity.Property(nameof(IBaseEntity.CreatedDate)).IsModified = false;
                        auditableEntity.Property(nameof(IBaseEntity.PlanId)).IsModified = false;
                        auditableEntity.Property(nameof(IBaseEntity.CreatedBy)).IsModified = false;
                    }
                }
            }
            var tenantEntitySet = ChangeTracker.Entries<ITenant>();
            if (tenantEntitySet != null)
            {


                // Audit set the audit information foreach record
                foreach (var auditableEntity in tenantEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted))
                {

                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                    }

                    if (auditableEntity.State == EntityState.Deleted)
                    {
                        auditableEntity.Entity.PlanId = _TenantService.Tenant;
                    }

                    if (auditableEntity.State == EntityState.Modified)
                    {
                        auditableEntity.Property(nameof(ITenant.PlanId)).IsModified = false;
                    }
                }
            }
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

    }


}