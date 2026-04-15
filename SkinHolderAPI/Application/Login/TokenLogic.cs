using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkinHolderAPI.Application.Login;

public interface ITokenLogic
{
    string GenerateToken(User user);
}

public class TokenLogic(IMapper mapper, IConfiguration config, ILogger<TokenLogic> logger) : BaseLogic(mapper, config, logger), ITokenLogic
{

    public string GenerateToken(User user)
    {
        _logger.LogInformation("GenerateToken llamado para userId={UserId}", user.Userid);

        try
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Userid.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            _logger.LogInformation("Token generado para userId={UserId}, expira={Expires}", user.Userid, expires);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando token para userId={UserId}", user.Userid);
            throw;
        }
    }
}
