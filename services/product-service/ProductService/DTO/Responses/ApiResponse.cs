namespace ProductService.DTO.Responses
{
    public class ApiResponse<T>
{
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Elapsed { get; set; }
        public string Error { get; set; } = string.Empty;
        public T? Data { get; set; }
}

}

//Isso garante uniformidade nas respostas, facilita logs e traz sempre Message, Data e Error.