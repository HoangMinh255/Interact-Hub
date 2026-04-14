public interface IPostRepository
{
    Task<IList<Post>> GetAll();
    Task<Post?> GetPostById(int id);
}