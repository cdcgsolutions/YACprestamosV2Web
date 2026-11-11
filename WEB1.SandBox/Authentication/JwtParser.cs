using System.Security.Claims;
using System.Text.Json;

namespace WEB1.SandBox.Authentication
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return claims;
            }

            var parts = jwt.Split('.');
            if (parts.Length < 2)
            {
                return claims;
            }

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null)
            {
                return claims;
            }

            foreach (var kvp in keyValuePairs)
            {
                var key = kvp.Key;
                var value = kvp.Value?.ToString() ?? string.Empty;

                if (key.EndsWith("nameidentifier", StringComparison.OrdinalIgnoreCase) || key == "nameid" || key == "sub")
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, value));
                }
                else if (key.EndsWith("/name", StringComparison.OrdinalIgnoreCase) || key == "name" || key == "unique_name")
                {
                    claims.Add(new Claim(ClaimTypes.Name, value));
                }
                else if (key.EndsWith("emailaddress", StringComparison.OrdinalIgnoreCase) || key == "email")
                {
                    claims.Add(new Claim(ClaimTypes.Email, value));
                }
                else if (key.EndsWith("role", StringComparison.OrdinalIgnoreCase) || key == "role")
                {
                    if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        var roles = JsonSerializer.Deserialize<string[]>(value);
                        if (roles != null)
                        {
                            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, value));
                    }
                }
                else
                {
                    claims.Add(new Claim(key, value));
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64.Replace('-', '+').Replace('_', '/'));
        }
    }
}
