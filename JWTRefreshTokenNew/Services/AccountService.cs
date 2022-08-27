using JWTRefreshTokenNew.Data;
using JWTRefreshTokenNew.Data.Entities;
using JWTRefreshTokenNew.Dtos;
using JWTRefreshTokenNew.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTRefreshTokenNew.Services
{
    public class AccountService: IAccountService
    {

        private readonly MyAuthContext _myAuthContext;
        private readonly TokenSettings _tokenSettings;
        public AccountService(MyAuthContext myAuthContext,
        IOptions<TokenSettings> tokenSettings)
        {
            _myAuthContext = myAuthContext;
            _tokenSettings = tokenSettings.Value;
        }
		private string CreateJwtToken(User user)
		{
			var symmetricSecurityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)
			);
			var credentials = new SigningCredentials(
				symmetricSecurityKey,
				SecurityAlgorithms.HmacSha256
			);

			var userCliams = new Claim[]{
		new Claim("email", user.Email),
		new Claim("password",user.Password),
		new Claim("phone", user.PhoneNumber),
	};

			var jwtToken = new JwtSecurityToken(
				issuer: _tokenSettings.Issuer,
				expires: DateTime.Now.AddSeconds(60),
				signingCredentials: credentials,
				claims: userCliams,
				audience: _tokenSettings.Audience
			);

			string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			return token;
		}

		//public async Task<TokenDto> GetAuthTokens(LoginDto login)
		//{
		//	//Note: demo purpose saved plain password to the database
		//	// checking the plain password
		//	// In real time application please make sure to save the hash password into the database
		//	// need to hash the password while comparing also
		//	User user = await _myAuthContext.User
		//	.Where(_ => _.Email.ToLower() == login.Email.ToLower() &&
		//	_.Password == login.Password).FirstOrDefaultAsync();

		//	if (user != null)
		//	{
		//		var token = new TokenDto
		//		{
		//			AccessToken = CreateJwtToken(user)
		//		};
		//		return token;
		//	}
		//	return null;
		//}
		private string CreateRefreshToken()
		{
			byte[] r = new byte[100];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(r);
			//var token1 = RandomNumberGenerator.GetBytes(64);
			var refreshtoken = Convert.ToBase64String(r);

			var tokenIsInUser = _myAuthContext.RefreshToken
			.Any(_ => _.Token == refreshtoken);

			if (tokenIsInUser)
			{
				return CreateRefreshToken();
			}
			return refreshtoken;
		}
		private async Task InsertRefreshToken(int userId, string refreshtoken)
		{
			var newRefreshToken = new RefreshToken
			{
				UserId = userId,
				Token = refreshtoken,
				ExpirationDate = DateTime.Now.AddDays(7)
			};
			_myAuthContext.RefreshToken.Add(newRefreshToken);
			await _myAuthContext.SaveChangesAsync();
		}
		public async Task<TokenDto> GetAuthTokens(LoginDto login)
		{

			User user = await _myAuthContext.User
			.Where(_ => _.Email.ToLower() == login.Email.ToLower() &&
			_.Password == login.Password).FirstOrDefaultAsync();

			if (user != null)
			{
				var accessToken = CreateJwtToken(user);
				var refreshToken = CreateRefreshToken();
				await InsertRefreshToken(user.Id, refreshToken);
				return new TokenDto
				{
					AccessToken = accessToken,
					RefreshToken = refreshToken
				};
			}
			return null;
		}
		public async Task<TokenDto> RenewTokens(RefreshTokenDto refreshToken)
		{
			var userRefreshToken = await _myAuthContext.RefreshToken
			.Where(_ => _.Token == refreshToken.Token
			&& _.ExpirationDate >= DateTime.Now).FirstOrDefaultAsync();

			if (userRefreshToken == null)
			{
				return null;
			}

			var user = await _myAuthContext.User
			.Where(_ => _.Id == userRefreshToken.UserId).FirstOrDefaultAsync();



			var newJwtToken = CreateJwtToken(user);
			var newRefreshToken = CreateRefreshToken();

			userRefreshToken.Token = newRefreshToken;
			userRefreshToken.ExpirationDate = DateTime.Now.AddMinutes(7);
			await _myAuthContext.SaveChangesAsync();

			return new TokenDto
			{
				AccessToken = newJwtToken,
				RefreshToken = newRefreshToken
			};
		}
	}
}
