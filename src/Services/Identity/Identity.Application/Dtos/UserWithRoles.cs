namespace Identity.Application.Dtos
{
    public class UserWithRoles
    {
        public UserViewModel User { get; set; }
        public List<string> Roles { get; set; }
    }
}
