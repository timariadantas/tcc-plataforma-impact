namespace ClientService.DTO.Requests
{

public class UpdateClientDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public bool Active { get; set; } = true; // opcional
}

}

//Não precisa do Id, porque ele vem da rota ([HttpPut("{id}")]).