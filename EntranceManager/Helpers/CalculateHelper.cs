using EntranceManager.Models;

namespace EntranceManager.Helpers
{
    public static class CalculateHelper
    {
        public static int CalculateResidents(int apartmentUsers, int numberOfChildren, bool includeChildren)
        {
            return includeChildren ? apartmentUsers + numberOfChildren : apartmentUsers;
        }

        public static decimal CalculateFeeForApartment(
             decimal totalFee,
             int totalApartments,
             int totalResidents,
             int apartmentResidentsCount,
             int numberOfChildren,
             int numberOfPets,
             FeeDetails details)
         {
            decimal baseApartmentFee = 0;
            decimal baseResidentFee = 0;

            if (totalApartments > 0)
            {
                baseApartmentFee = (totalFee / totalApartments) * details.ApartmentRatio;
            }

            if (totalResidents > 0 && apartmentResidentsCount > 0)
            {
                baseResidentFee = (totalFee * ((decimal)apartmentResidentsCount / totalResidents)) * details.ResidentRatio;
            }

            decimal total = baseApartmentFee + baseResidentFee;

                if (details?.IsFeeForCleaning == true && numberOfPets > 0)
                    total *= (1 + details.PetFeePercentage * numberOfPets);

                if (details?.IsFeeWithChildrenReduction == true && numberOfChildren > 0)
                {
                    decimal discount = details.ChildDiscountPercentage * numberOfChildren;
                    total *= Math.Max(0, 1 - discount);
                }

                return Math.Round(total, 2);
        }

        public static Dictionary<int, decimal> NormalizeFees(Dictionary<int, decimal> rawFees, decimal totalAmount)
        {
            var totalRaw = rawFees.Values.Sum();

            if (totalRaw == 0 || totalAmount == 0)
                return rawFees.ToDictionary(kvp => kvp.Key, kvp => 0m);

            return rawFees.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round((kvp.Value / totalRaw) * totalAmount, 2)
            );
        }
    }
}
