using MapsterMapper;
using SkinHolderAPI.Application.Login;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Users;
using SkinHolderAPI.DTOs.Login;
using System.Security.Cryptography;
using System.Text;

namespace SkinHolderAPI.Application.Users;

public interface IUserLogic
{
    Task<LoginResultDto?> LoginAsync(LoginDto loginDto);
}

public class UserLogic(IUserDataService userDataService, ITokenLogic tokenLogic, IConfiguration config, IMapper mapper, ILogger<UserLogic> logger) : BaseLogic(mapper, config, logger), IUserLogic
{
    private readonly IUserDataService _userDataService = userDataService;
    private readonly ITokenLogic _tokenLogic = tokenLogic;

    public async Task<LoginResultDto?> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("LoginAsync: intento de login para usuario={Username}", loginDto.Username);

        try
        {
            var user = await _userDataService.GetByEmailAsync(loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("LoginAsync: usuario no encontrado para username={Username}", loginDto.Username);
                return null;
            }

            if (user.Passwordhash != ComputeSha512(loginDto.Password))
            {
                _logger.LogWarning("LoginAsync: contraseña incorrecta para username={Username}", loginDto.Username);
                return null;
            }

            var token = _tokenLogic.GenerateToken(user);

            _logger.LogInformation("LoginAsync: login exitoso para userId={UserId}, username={Username}", user.Userid, user.Username);

            return new LoginResultDto
            {
                Token = token,
                Username = user.Username,
                UserId = user.Userid
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en LoginAsync para username={Username}", loginDto.Username);
            throw;
        }
    }

    #region Utils
    public static string ComputeSha512(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA512.HashData(bytes);
        return Convert.ToHexString(hash);
    }
    #endregion
}
