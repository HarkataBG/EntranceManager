using EntranceManager.Models;
using EntranceManager.Models.Mappers;

namespace AspNetCoreDemo.Helpers
{
    public class ModelMapper
    {
        public Apartment Map(ApartmentDto dto)
        {
            Apartment apartment = new Apartment();
            apartment.Floor = dto.Floor;
            apartment.Number = dto.Number;
            apartment.OwnerUserId = dto.OwnerUserId;
            apartment.EntranceId = dto.EntranceId;

            return apartment;
        }
    }
}
