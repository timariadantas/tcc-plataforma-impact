
namespace ClientService.DTO.Requests
{
    public class CreateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
    }

}