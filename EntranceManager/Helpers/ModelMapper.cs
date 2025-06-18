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
            apartment.NumberOfChildren = dto.NumberOfChildren;
            apartment.NumberOfPets = dto.NumberOfPets;

            return apartment;
        }

        public ApartmentResponseDto Map(Apartment apartment, int numberOfLivingPeople)
        {
            return new ApartmentResponseDto
            {
                Id = apartment.Id,
                Floor = apartment.Floor,
                Number = apartment.Number,
                NumberOfLivingPeople = numberOfLivingPeople,
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
                Residents = apartment.ApartmentUsers?
                   .Select(au => new ResidentDto
                   {
                       Id = au.User.Id,
                       Username = au.User.Username
                   })
                   .ToList() ?? new List<ResidentDto>(),
                Fees = apartment.ApartmentFees?
                   .Select(af => new FeeSummaryDto
                   {
                       Id = af.Id,
                       FeeId = af.FeeId,
                       Name = af.Fee.Name,
                       Description = af.Fee.Description,
                       Amount = af.AmountForApartment, 
                       IsPaid = af.IsPaid,
                       PaymentDate = af.PaymentDate,
                       AmountForApartment = af.AmountForApartment,
                       AmountAlreadyPaid = af.AmountAlreadyPaid
                   })
                    .ToList() ?? new List<FeeSummaryDto>(),
                NumberOfChildren = apartment.NumberOfChildren,
               NumberOfPets = apartment.NumberOfPets
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
            entrance.CountChildrenAsResidents = dto.CountChildrenAsResidents;

            return entrance;
        }

        public EntranceResponseDto Map(Entrance entrance)
        {
            return new EntranceResponseDto
            {
                Id = entrance.Id,
                City = entrance.City,
                Address = entrance.Address,
                PostCode = entrance.PostCode,
                EntranceName = entrance.EntranceName,
                CountChildrenAsResidents = entrance.CountChildrenAsResidents,

                Manager = entrance.ManagerUser != null
                 ? new ManagerDto
                 {
                     Id = entrance.ManagerUser.Id,
                     Username = entrance.ManagerUser.Username
                 }
                 : new ManagerDto
                 {
                     Id = 0,
                     Username = string.Empty
                 },

                Apartments = entrance.Apartments?.Select(a => new ApartmentSummaryDto
                {
                    Id = a.Id,
                    Number = a.Number,
                    Floor = a.Floor
                }).ToList() ?? new List<ApartmentSummaryDto>(),

                Residents = entrance.EntranceUsers?
                 .Where(eu => eu.User != null)
                 .Select(eu => new ResidentDto
                 {
                     Id = eu.User.Id,
                     Username = eu.User.Username
                 })
                 .ToList() ?? new List<ResidentDto>()
            };
        }

        public Fee Map(FeeDto dto)
        {
            return new Fee
            {
                Name = dto.Name,
                Description = dto.Description,
                Amount = dto.Amount,
                EntranceId = dto.EntranceId
            };
        }

        public FeeResponseDto Map(Fee fee)
        {
            return new FeeResponseDto
            {
                Id = fee.Id,
                Name = fee.Name,
                Description = fee.Description,
                Amount = fee.Amount,
                Entrance = new EntranceSummaryDto
                {
                    Id = fee.Entrance?.Id ?? 0,
                    City = fee.Entrance?.City ?? string.Empty,
                    Address = fee.Entrance?.Address ?? string.Empty,
                    PostCode = fee.Entrance?.PostCode ?? 0,
                    EntranceName = fee.Entrance?.EntranceName ?? string.Empty
                },
                ApartmentFees = fee.ApartmentFees?
                 .Where(af => af.Apartment != null)
                 .Select(af => new ApartmentFeeDto
                 {
                     Id = af.Id,
                     ApartmentId = af.ApartmentId,
                     IsPaid = af.IsPaid,
                     PaymentDate = af.PaymentDate
                 })
                 .ToList() ?? new List<ApartmentFeeDto>()
            };
        }

        public void Map(FeeDto dto, Fee? fee = null)
        {
            if (fee == null)
                fee = new Fee();

            fee.Name = dto.Name;
            fee.Description = dto.Description;
            fee.Amount = dto.Amount;
            fee.EntranceId = dto.EntranceId;
        }
    }
}
