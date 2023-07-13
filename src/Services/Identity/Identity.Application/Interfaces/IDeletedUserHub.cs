namespace Identity.Application.Interfaces
{
    public interface IDeletedUserHub
    {
        Task DeletedUsers(string userId);
    }
}
