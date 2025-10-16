using ClientService.Domain;
using ClientService.Domain.Validation;
using ClientService.Storage;
using System;
using System.Collections.Generic;

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

        // CREATE
        public void Create(Client client)
        {
            // Validações
            foreach (var validator in _validators)
            {
                validator.Validate(client);
            }

            _storage.Create(client); // síncrono
        }

        // GET BY ID
        public Client? GetById(string id)
        {
            return _storage.GetById(id); // síncrono
        }

        // GET ALL
        public List<Client> GetAll()
        {
            return _storage.GetAll(); // síncrono
        }

        // DELETE
        public void Delete(string id)
        {
            var client = _storage.GetById(id);
            if (client == null)
                throw new ArgumentException("Cliente não encontrado");

            _storage.Delete(id);
        }

        // UPDATE
        public void Update(Client client)
        {
            // Validações
            foreach (var validator in _validators)
            {
                validator.Validate(client);
            }

            _storage.Update(client); // envia apenas os campos que o usuário pode alterar


        }

    }
}
