public interface IUserRepository
{
    Task<IList<AppUser>> GetAll();
    Task<AppUser?> GetUserById(string id);
}