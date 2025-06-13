using AutoMapper;
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

public class UserLogic(IUserDataService userDataService, ITokenLogic tokenLogic, IConfiguration config, IMapper mapper) : BaseLogic(mapper, config), IUserLogic
{
    private readonly IUserDataService _userDataService = userDataService;
    private readonly ITokenLogic _tokenLogic = tokenLogic;

    public async Task<LoginResultDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userDataService.GetByEmailAsync(loginDto.Username);

        if (user == null) return null;

        if (user.PasswordHash != ComputeSha512(loginDto.Password)) return null;

        var token = _tokenLogic.GenerateToken(user);

        return new LoginResultDto
        {
            Token = token,
            Username = user.Username
        };
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
