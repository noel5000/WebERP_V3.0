using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebERPV3.Entities
{
    public class InvoiceDetail : BaseEntity
    {
        public InvoiceDetail() { }

        public InvoiceDetail(InvoiceDetail newDetail)
        {
            this.Id = newDetail.Id;
            this.ProductId = newDetail.ProductId;
            this.Product = newDetail.Product ?? null;
            this.IsDeleted = newDetail.IsDeleted;
            this.WarehouseId = newDetail.WarehouseId ?? null;
            this.Quantity = newDetail.Quantity;
            this.Comments = newDetail.Comments;
            this.CreatedBy = newDetail.CreatedBy;
            this.InvoiceId = newDetail.InvoiceId;
            this.Date = newDetail.Date;
            this.CreatedDate = newDetail.CreatedDate;
            this.ModifiedDate = newDetail.ModifiedDate;
            this.UpdatedBy = newDetail.UpdatedBy;
            this.Amount = newDetail.Amount;
            this.PrincipalCurrencyAmount = newDetail.PrincipalCurrencyAmount;
            this.BeforeTaxesAmount = newDetail.BeforeTaxesAmount;
            this.TaxesAmount = newDetail.TaxesAmount;
            this.TotalAmount = newDetail.TotalAmount;
            this.Unit = newDetail.Unit ?? null;
            this.UnitId = newDetail.UnitId ?? null;
            this.DiscountAmount = newDetail.DiscountAmount;
            this.CreditNoteAmount = newDetail.CreditNoteAmount;
            this.ParentId = newDetail.ParentId;
            this.SaveRegister = newDetail.SaveRegister;
            this.DiscountRate = newDetail.DiscountRate;
            this.Free = newDetail.Free;
            this.SellerRate = newDetail.SellerRate;
            this.Cost = newDetail.Cost;
            this.ReturnAmount = newDetail.ReturnAmount;
            this.Defective = newDetail.Defective;
        }
        [NotMapped]
        public bool Defective { get; set; }
        [NotMapped]
        public decimal ReturnAmount { get; set; }


        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellerRate { get; set; } = 0;
        public decimal Cost { get; set; }
        [NotMapped]
        public override string TranslationData { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxesAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Free { get; set; }
        public DateTime Date { get; set; }

        public long InvoiceId { get; set; }

        public decimal BeforeTaxesAmount { get; set; }
        [MaxLength(200)]
        public string Comments { get; set; }

        public decimal PrincipalCurrencyAmount { get; set; }
        public decimal DiscountRate { get; set; }






        public Nullable<long> WarehouseId { get; set; }



        public decimal DiscountAmount { get; set; }
        public decimal CreditNoteAmount { get; set; }

        public Nullable<long> ParentId { get; set; }

        public Nullable<long> UnitId { get; set; }

        public bool Equals(InvoiceDetail other)
        {
            return (this.Id == other.Id && this.ProductId == other.ProductId && this.Quantity == other.Quantity && this.Amount == other.Amount && this.Cost == other.Cost &&
                this.UnitId == other.UnitId && this.TaxesAmount == other.TaxesAmount && this.BeforeTaxesAmount == other.BeforeTaxesAmount && this.TotalAmount == other.TotalAmount &&
                this.InvoiceId == other.InvoiceId && this.CreditNoteAmount == other.CreditNoteAmount && this.DiscountAmount == other.DiscountAmount && this.DiscountRate == other.DiscountRate
                && this.Free == other.Free && this.ParentId == other.ParentId && this.WarehouseId == other.WarehouseId &&
                this.IsDeleted == other.IsDeleted && this.Date == other.Date && this.CreatedDate == other.CreatedDate);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                long hashCode = 13;
                hashCode = (hashCode * 397) ^ this.Id;

                hashCode = (hashCode * 397) ^ this.ProductId;
                hashCode = (hashCode * 397) ^ this.InvoiceId;



                hashCode = this.UnitId.HasValue ? (hashCode * 397) ^ this.UnitId.Value : hashCode;
                hashCode = this.ParentId.HasValue ? (hashCode * 397) ^ this.ParentId.Value : hashCode;
                hashCode = this.WarehouseId.HasValue ? (hashCode * 397) ^ this.WarehouseId.Value : hashCode;
                hashCode = (hashCode * 397) ^ Convert.ToInt32(this.IsDeleted);

                var hashCodeDecimal = this.Quantity.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal;

                var hashCodeDecimal2 = this.Amount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal2;

                var hashCodeDecimal3 = this.BeforeTaxesAmount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal3;

                var hashCodeDecimal4 = this.TaxesAmount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal4;

                var hashCodeDecimal5 = this.TotalAmount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal5;

                var hashCodeDecimal6 = this.DiscountAmount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal6;

                var hashCodeDecimal7 = this.CreditNoteAmount.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal7;

                var hashCodeDecimal8 = this.DiscountRate.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal8;

                var hashCodeDecimal9 = this.Cost.GetHashCode();
                hashCode = hashCode ^ hashCodeDecimal9;

                return Convert.ToInt32( hashCode);
            }
        }


        [ForeignKey("ProductId")]
        public Product Product { get; set; }



        [ForeignKey("UnitId")]
        public Unit Unit { get; set; }


        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        [NotMapped]
        public bool SaveRegister { get; set; } = true;

    }

}

