using EntranceManager.Models;
using System.Threading.Tasks;

namespace EntranceManager.Repositories.Contracts
{
    public interface IFeeRepository
    {
        Task<IEnumerable<Fee>> GetAllFeeDetailsAsync(User currentUser);
        Task<Fee> GetFeeDetailsByIdAsync(int id);

        Task<Fee> GetByIdAsync(int id);
        Task AddAsync(Fee fee);
        Task UpdateAsync(Fee fee);
        Task DeleteAsync(int feeId);
    }
}
