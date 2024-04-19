using EMS.BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EMS.ClientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _localStorageService;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(LocalStorageService storageService)
        {
            _localStorageService = storageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await _localStorageService.GetToken();
            if (string.IsNullOrEmpty(stringToken))
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if (deserializeToken is null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var getUserClaims = DecryptToken(deserializeToken.Token!);
            if (getUserClaims is null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();

            if (userSession.Token is not null || userSession.RefreshToken is not null)
            {
                var serializeSession = Serializations.SezializeObj(userSession);
                await _localStorageService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            }
            else
            {
                await _localStorageService.RemoveToken();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var email = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var role = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            return new CustomUserClaims(userId!, name!, email!, role!);
        }

        private static ClaimsPrincipal SetClaimsPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null)
                return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, claims.Id),
                    new(ClaimTypes.Name, claims.Name),
                    new(ClaimTypes.Email, claims.Email),
                    new(ClaimTypes.Role, claims.Role)
                }));
        }
    }
}