using JWTRefreshTokenNew.Dtos;
using System.Threading.Tasks;

namespace JWTRefreshTokenNew.Services
{
    public interface IAccountService
    {
        Task<TokenDto> GetAuthTokens(LoginDto login);
        Task<TokenDto> RenewTokens(RefreshTokenDto refreshToken);
    }
}
