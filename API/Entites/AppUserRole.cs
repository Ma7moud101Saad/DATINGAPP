using Microsoft.AspNetCore.Identity;

namespace API.Entites
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public AppUser AppUser { get; set; }
        public AppRole AppRole { get; set; }
    }
}
