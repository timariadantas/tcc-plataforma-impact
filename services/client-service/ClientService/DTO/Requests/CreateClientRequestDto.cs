
namespace ClientService.DTO.Requests
{
    public class CreateClientRequestDto
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime Birthdate { get; set; }
    }

}