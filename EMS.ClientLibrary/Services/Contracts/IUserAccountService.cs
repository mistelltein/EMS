using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Responses;
using EMS.Server;

namespace EMS.ClientLibrary.Services.Contracts
{
    public interface IUserAccountService
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<WeatherForecast[]> GetWeatherForecasts();
    }
}