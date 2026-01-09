namespace ClientService.DTO.Responses
{
    public class ClientResponseDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime Birthdate { get; set; }
        public bool Active { get; set; }
       // public DateTime? CreatedAt { get; set; } = null;  Em caso de Historico e Auditoria 
       // public DateTime? UpdatedAt { get; set; } = null;
    }

}
