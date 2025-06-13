namespace EntranceManager.Helpers
{
    public static class CalculateHelper
    {
        public static int CalculateResidents(int apartmentUsers, int numberOfChildren, bool includeChildren)
        {
            return includeChildren ? apartmentUsers + numberOfChildren : apartmentUsers;
        }
    }
}
