using EntranceManager.Models;
using EntranceManager.Models.Mappers;

namespace AspNetCoreDemo.Helpers
{
    public class ModelMapper
    {
        public Apartment Map(ApartmentDto dto, Apartment? apartment = null)
        {
            if (apartment == null)
                apartment = new Apartment();

            apartment.Floor = dto.Floor;
            apartment.Number = dto.Number;
            apartment.OwnerUserId = dto.OwnerUserId;
            apartment.EntranceId = dto.EntranceId;

            return apartment;
        }

        public ApartmentResponseDto Map(Apartment apartment)
        {
            return new ApartmentResponseDto
            {
                Id = apartment.Id,
                Floor = apartment.Floor,
                Number = apartment.Number,
                NumberOfLivingPeople = apartment.ApartmentUsers?.Count ?? 0,
                Owner = new OwnerDto
                {
                    Id = apartment.OwnerUser.Id,
                    Username = apartment.OwnerUser.Username
                },
                Entrance = new EntranceSummaryDto
                {
                    Id = apartment.Entrance.Id,
                    City = apartment.Entrance.City,
                    Address = apartment.Entrance.Address,
                    PostCode = apartment.Entrance.PostCode,
                    EntranceName = apartment.Entrance.EntranceName
                },
                Residents = apartment.ApartmentUsers
           .Select(au => new ResidentDto
           {
               Id = au.User.Id,
               Username = au.User.Username
           })
           .ToList()
            };
        }

        public Entrance Map(EntranceDto dto, Entrance? entrance = null)
        {
            if (entrance == null)
                entrance = new Entrance();

            entrance.City = dto.City;
            entrance.EntranceName = dto.EntranceName;
            entrance.Address = dto.Address;
            entrance.PostCode = dto.PostCode;

            return entrance;
        }

        public EntranceResponseDto Map(Entrance entrance)
        {
            return new EntranceResponseDto
            {
                Id = entrance.Id,
                City = entrance.City,
                Address = entrance.Address,
                EntranceName = entrance.EntranceName,
                Manager = new ManagerDto
                {
                    Id = entrance.ManagerUser.Id,
                    Username = entrance.ManagerUser.Username
                },
                Apartments = entrance.Apartments?.Select(a => new ApartmentSummaryDto
                {
                    Id = a.Id,
                    Number = a.Number,
                    Floor = a.Floor
                }).ToList() ?? new List<ApartmentSummaryDto>(),

                Residents = entrance.EntranceUsers?
                .Select(eu => new ResidentDto
                {
                    Id = eu.User.Id,
                    Username = eu.User.Username
                })
                .ToList() ?? new List<ResidentDto>()
                    };
                }

    }
}
