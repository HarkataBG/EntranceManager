namespace EntranceManager.Models
{
    public class FeeDetails
    {
        public decimal ApartmentRatio { get; set; }
        public decimal ResidentRatio { get; set; }
        public bool IsFeeForCleaning { get; set; }
        public bool IsFeeWithChildrenReduction { get; set; }
        public decimal PetFeePercentage { get; set; }
        public decimal ChildDiscountPercentage { get; set; }
        public bool Normalize { get; set; }
    }
}
