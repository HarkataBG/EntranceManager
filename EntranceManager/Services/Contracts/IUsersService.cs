namespace EntranceManager.Services.Contracts
{
    public interface IUsersService
    {
        Task PromoteToManagerAsync(int userId, int entranceId);
    }
}
