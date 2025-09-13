using ClientService.Domain;

public interface IClientStorage
{
    void Create(Client client);
    Client? GetById(string id);
    List<Client> GetAll();
    void Delete(string id);
    void Update(Client client);
}


