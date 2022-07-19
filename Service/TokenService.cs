using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RegAuthApiDemo.Domain;

namespace RegAuthApiDemo.Service
{
	public class TokenService : ITokenService
	{
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly UserManager<User> _userManager;

		public TokenService(IConfiguration configuration, UserManager<User> userManager)
		{
            this._configuration = configuration;
            this._symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"])); ;
            _userManager = userManager;
		}

        public string CreateToken(User appUser)
        {
            var userRole = _userManager.GetRolesAsync(appUser);

            var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, Guid.NewGuid().ToString())
               

            };

            var authCred = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var tokenDescriotor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaim),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = authCred,
                Issuer = _configuration["Token:Issuer"],
                Audience = _configuration["Token:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var identity = new ClaimsIdentity(authClaim, "JTW");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);


            var token = tokenHandler.CreateToken(tokenDescriotor);

            return tokenHandler.WriteToken(token);
        }
    }
}

