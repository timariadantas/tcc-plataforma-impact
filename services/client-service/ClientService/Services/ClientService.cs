using ClientService.Domain;
using ClientService.Domain.Validation;
using ClientService.Service;
using ClientService.Storage;


namespace ClientService.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientStorage _storage;
        private readonly List<IValidationStrategy<Client>> _validators;

        public ClientService(IClientStorage storage)
        {
            _storage = storage;
            _validators = new List<IValidationStrategy<Client>>
        {
            new EmailValidation(),
            new NameValidation()
        };
        }

        public void Create(Client client)
        {
            foreach (var v in _validators) v.Validate(client);

            client.CreatedAt = DateTime.UtcNow;
            client.UpdatedAt = DateTime.UtcNow;

            _storage.Create(client);

        }
        public Client? GetById(string id) => _storage.GetById(id);

        public void Delete(string id) => _storage.Delete(id);
    

        public List<Client> GetAll() => _storage.GetAll();
    

        
    }


}


