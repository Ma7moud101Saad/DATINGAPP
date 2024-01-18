using API.Entites;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateToken(AppUser appUser);
    }
}
