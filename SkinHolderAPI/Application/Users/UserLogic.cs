using MapsterMapper;
using SkinHolderAPI.Application.Login;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Users;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.DTOs.Users;
using System.Security.Cryptography;
using System.Text;

namespace SkinHolderAPI.Application.Users;

public interface IUserLogic
{
    Task<LoginResultDto?> LoginAsync(LoginDto loginDto);
    Task<UserInfoDto?> GetUserInfoAsync(int userId);
    Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(int userId, DeleteAccountDto dto);
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

    public async Task<UserInfoDto?> GetUserInfoAsync(int userId)
    {
        _logger.LogInformation("GetUserInfoAsync: obteniendo info para userId={UserId}", userId);

        try
        {
            var user = await _userDataService.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("GetUserInfoAsync: usuario no encontrado para userId={UserId}", userId);
                return null;
            }

            return new UserInfoDto
            {
                UserId = user.Userid,
                Username = user.Username,
                CreatedAt = user.Createdat
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetUserInfoAsync para userId={UserId}", userId);
            throw;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        _logger.LogInformation("ChangePasswordAsync: intento de cambio de contraseña para userId={UserId}", userId);

        try
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return (false, "Las contraseñas no pueden estar vacías.");

            if (dto.NewPassword.Length < 6)
                return (false, "La nueva contraseña debe tener al menos 6 caracteres.");

            var user = await _userDataService.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("ChangePasswordAsync: usuario no encontrado para userId={UserId}", userId);
                return (false, "Usuario no encontrado.");
            }

            if (user.Passwordhash != ComputeSha512(dto.CurrentPassword))
            {
                _logger.LogWarning("ChangePasswordAsync: contraseña actual incorrecta para userId={UserId}", userId);
                return (false, "La contraseña actual es incorrecta.");
            }

            if (ComputeSha512(dto.CurrentPassword) == ComputeSha512(dto.NewPassword))
                return (false, "La nueva contraseña no puede ser igual a la actual.");

            var newHash = ComputeSha512(dto.NewPassword);
            var success = await _userDataService.UpdatePasswordAsync(userId, newHash);

            if (!success)
                return (false, "Error al actualizar la contraseña.");

            _logger.LogInformation("ChangePasswordAsync: contraseña actualizada para userId={UserId}", userId);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ChangePasswordAsync para userId={UserId}", userId);
            throw;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(int userId, DeleteAccountDto dto)
    {
        _logger.LogInformation("DeleteAccountAsync: intento de eliminación de cuenta para userId={UserId}", userId);

        try
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                return (false, "La contraseña no puede estar vacía.");

            var user = await _userDataService.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("DeleteAccountAsync: usuario no encontrado para userId={UserId}", userId);
                return (false, "Usuario no encontrado.");
            }

            if (user.Passwordhash != ComputeSha512(dto.CurrentPassword))
            {
                _logger.LogWarning("DeleteAccountAsync: contraseña incorrecta para userId={UserId}", userId);
                return (false, "La contraseña es incorrecta.");
            }

            var success = await _userDataService.DeactivateUserAsync(userId);

            if (!success)
                return (false, "Error al desactivar la cuenta.");

            _logger.LogInformation("DeleteAccountAsync: cuenta desactivada para userId={UserId}", userId);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeleteAccountAsync para userId={UserId}", userId);
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
