using DatingApp.Entities;
using System.Threading.Tasks;

namespace DatingApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
