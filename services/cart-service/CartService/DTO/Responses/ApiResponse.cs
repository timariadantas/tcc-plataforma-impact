namespace CartService.DTO.Responses
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int Elapsed { get; set; }
        public string Error { get; set; } = string.Empty;
        public T? Data { get; set; } 
    }
}
