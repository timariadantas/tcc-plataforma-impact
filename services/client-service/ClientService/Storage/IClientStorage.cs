using ClientService.Domain;

namespace ClientService.Storage
{
    public interface IClientStorage
    {
        void Create(Client client);
        Client? GetById(string id);
        List<Client> GetAll();
        void Delete(string id);
}

}

