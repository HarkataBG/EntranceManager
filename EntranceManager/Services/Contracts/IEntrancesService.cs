using EntranceManager.Models;
using EntranceManager.Models.Mappers;

namespace EntranceManager.Services
{
    public interface IEntranceService
    {
        Task<Entrance?> GetEntranceByIdAsync(int id);

        Task AddEntranceAsync(EntranceDto dto);

        Task UpdateEntranceAsync(int id, EntranceDto dto);

        Task DeleteEntranceAsync(int id);

        Task<IEnumerable<EntranceResponseDto>> GetAllEntrancesDetailsAsync(string username);

        Task<EntranceResponseDto> GetEntranceDetailsByIdAsync(int id);

        Task AddUserToEntranceAsync(int entranceId, int userId);

        Task RemoveUserFromEntrance(int apartmentId, int userId);
    }
}