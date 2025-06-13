using EntranceManager.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EntranceManager.Services.Contracts
{
    public interface IUsersService
    {
        Task PromoteToManagerAsync(int userId, int entranceId);
        Task DemoteFromManagerAsync(int entranceId);
        Task<User> GetByUsernameAsync(string username);
        Task GetAuthorizedUserForEntranceAsync(ClaimsPrincipal userPrincipal, int entranceId);
        Task GetAuthorizedUserForApartmentAsync(ClaimsPrincipal userPrincipal, Apartment apartment);
    }
}
