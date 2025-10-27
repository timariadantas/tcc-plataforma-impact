namespace ClientService.DTO.Responses
{

    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Elapsed { get; set; } = 0;
        public string Error { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}

