using ClientService.Domain;



namespace ClientService.Service
{
    public interface IClientService
    {
        void Create(Client client);
        Client? GetById(string id);
        List<Client> GetAll();
        void Delete(string id);
    }
}

