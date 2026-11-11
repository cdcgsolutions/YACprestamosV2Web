using System.ComponentModel.DataAnnotations;

namespace WEB1.SandBox.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario o correo es requerido.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Value { get; set; }
        public List<string>? Errors { get; set; }
    }
}
