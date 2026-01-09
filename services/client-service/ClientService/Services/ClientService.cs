using ClientService.Domain;
using ClientService.Domain.Validation;
using ClientService.Logging;
using ClientService.Storage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ClientService.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientStorage _storage;
        private readonly LoggerService _logger;


        public ClientService(IClientStorage storage , LoggerService logger)
        {
            _storage = storage;
            _logger = logger;
        }

        
        public Client Create(string name, string surname, string email, DateTime birthdate)
        {
           _logger.Log($"Iniciando criação de cliente: {name} {surname}");

           try
            {
                var client = Client.CreateNew(name, surname, email,  birthdate);
                _storage.Create(client);
                _logger.Log($"Cliente criado com sucesso ! Id={client.Id}");
                return client;
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao criar Cliente: {ex.GetType().Name} - {ex.Message} ");
                _logger.Log(ex.StackTrace ?? "Sem strack trace");
                throw;
            }
        }

        
        public Client? GetById(string id)
        {
             _logger.Log($" Buscando cliente por id: {id}");
                return _storage.GetById(id); 
        }

        
        public List<Client> GetAll()
        {
            _logger.Log($"Buscando todos os clintes");
            return _storage.GetAll(); 
        }

      
        
        public void Update(string id, string name ,string surname, string email , DateTime birthdate )
        {
            _logger.Log($"Atualizando Cliente {id}");

            var client = _storage.GetById(id);
            if (client == null)
            {
                _logger.Log($"Client {id} não encontrado para atualização");
                throw new ArgumentException("Cliente não encontrado");

            }
            client.UpdatePersonalData(name, surname, email, birthdate);
            _storage.Update(client);

            _logger.Log($"Cliente{id} atualizado com sucesso");
        }
          // Apagar fisicamente o registro do banco.
        public void Delete(string id)  
        {
             _logger.Log($"Solicitada exclusão do cliente {id}");

            var client = _storage.GetById(id);
            if (client == null)
            {
            _logger.Log($"Cliente {id} não encontrado para exclusão");
                throw new ArgumentException("Cliente não encontrado");
        }
            _storage.Delete(id);
            _logger.Log($"Cliente {id} excluído com sucesso");
        }

         // DEACTIVATE   -- O cliente deixa de estar ativo no negócio, mas continua existindo.
        public void Deactivate(string id)
        {
            _logger.Log($"[ClientService] Desativando cliente {id}");

            var client = _storage.GetById(id);
            if (client == null)
            {
                _logger.Log($"[ClientService] Cliente {id} não encontrado para desativação");
                throw new ArgumentException("Cliente não encontrado");
            }

            client.Deactivate();
            _storage.Update(client);

            _logger.Log($"[ClientService] Cliente {id} desativado com sucesso");
        }

        public void Update(string name, string surname, string email, DateTime birthdate)
        {
            throw new NotImplementedException();
        }
    }

    }

