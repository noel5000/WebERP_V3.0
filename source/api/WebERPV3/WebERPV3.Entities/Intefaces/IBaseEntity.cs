namespace WebERPV3.Entities
{
    public interface IBaseEntity: IEntityTranslate, ITenant
    {
        
        long Id { get; set; }
        string? CreatedBy { get;set; }
        string? UpdatedBy { get;set; }
        bool IsDeleted { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }

    }

    public interface ITenant 
    {
        string PlanId { get; set; }
    }
}