using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace WEB1.SandBox.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
        private const string TokenKey = "authToken";
        private ClaimsPrincipal? _cachedUser;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_cachedUser != null)
            {
                return new AuthenticationState(_cachedUser);
            }

            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = JwtParser.ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                _cachedUser = new ClaimsPrincipal(identity);

                return new AuthenticationState(_cachedUser);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            _cachedUser = new ClaimsPrincipal(identity);

            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            }
            catch
            {
                // Ignorar si JS no está listo aún
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            _cachedUser = null;
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            }
            catch
            {
                // Ignorar si JS no está listo aún
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
            }
            catch
            {
                return null;
            }
        }
    }
}
