using System.Text.Json.Serialization;

namespace WEB1.Module.DTOs
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("message")]
        public string? Message 
        { 
            get => ErrorMessage; 
            set => ErrorMessage = value; 
        }

        public int StatusCode { get; set; } = 200;
        public List<string>? Errors { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("message")]
        public string? Message 
        { 
            get => ErrorMessage; 
            set => ErrorMessage = value; 
        }

        public int StatusCode { get; set; } = 200;
        public List<string>? Errors { get; set; }
    }
}
