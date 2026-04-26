public class UserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IList<AppUser>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }
    public async Task<AppUser?> GetUserById(string id)
    {
        return await _userRepository.GetUserById(id);
    }
}