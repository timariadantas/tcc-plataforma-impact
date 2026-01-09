namespace ClientService.DTO.Requests
{

public class UpdateClientRequestDto
{
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime Birthdate { get; set; }
   // public bool Active { get; set; } = true; // opcional
}

}

//Não precisa do Id, porque ele vem da rota ([HttpPut("{id}")]).