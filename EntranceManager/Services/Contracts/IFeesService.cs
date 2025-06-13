using EntranceManager.Models;
using EntranceManager.Models.Mappers;

namespace EntranceManager.Services.Contracts
{
    public interface IFeesService
    {
        Task<IEnumerable<FeeResponseDto>> GetAllFeeDetailsAsync();
        Task<FeeResponseDto?> GetFeeDetailsByIdAsync(int id);
        Task<Fee> GetByIdAsync(int id);
        Task CreateFeeAsync(FeeDto dto);
        Task UpdateFeeAsync(int id, FeeDto dto);
        Task DeleteFeeAsync(int id);
    }
}
