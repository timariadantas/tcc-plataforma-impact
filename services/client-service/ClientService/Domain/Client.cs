
using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NUlid;
using ClientService.Domain.Validation;


namespace ClientService.Domain
{
    public class Client
    {
        public string Id { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }
        public DateTime Birthdate { get; private set; }
        public DateTime CreatedAt { get; } // histórico
        public DateTime UpdatedAt { get; private set; } // preenchido pelo banco
        public bool Active { get; private set; }

        // Construtor privado impedi new Client (fora do dominio).
        private Client(
            string id,
            string name,
            string surname,
            string email,
            DateTime birthdate,
            DateTime creatdAt,
            DateTime updatedAt,
            bool active)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            Birthdate = birthdate;
            CreatedAt = creatdAt; 
            UpdatedAt = updatedAt;
            Active = active;
        }

        // Cria cliente novo e valida as regras do negócio . Só a domain + service pode criar.
        internal static Client CreateNew(string name, string surname, string email, DateTime birthdate)
        {
            var client = new Client(
                NUlid.Ulid.NewUlid().ToString(),
                name,
                surname,
                email,
                birthdate,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true

            );
            ClientValidation.Validate(client);
            return client;
        }

        // Só reconstrói o objeto na memória, Porque os dados já foram validados antes e persistidos.
        internal static Client Restore(
        string id,
        string name,
        string surname,
        string email,
        DateTime birthdate,
        DateTime createdAt,
        DateTime updatedAt,
        bool active)
        {
            return new Client(id, name, surname, email, birthdate, createdAt, updatedAt, active);
        }

        // Mudar o estado de um cliente é regra de negócio, impedir desativação indevida.
        public void Deactivate()
        {
            Active = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePersonalData(string name, string surname, string email, DateTime birthdate)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Birthdate = birthdate;
            UpdatedAt = DateTime.UtcNow;

            ClientValidation.Validate(this);
        }


    }

}
// Tópicos importantes !
// A Domain não é uma camada técnica.Ela é o modelo do seu negócio.
// Esse modelo existe para garantir que seus dados sejam sempre válidos, consistentes e sob controle — do nascimento até a persistência.
// estados inválidos, se chama “Fail fast”.
// private : protege o estado do objeto contra mudanças inválidas vindas de fora.
// Boa prática: o identificador (ULID/UUID) deve ser gerado na aplicação (Domain) e não no banco.
// O Domain cria o ID (ULID).O banco só armazena.
/////
/// 
// DDD + Clean Architecture:
// O domínio manda, a infraestrutura obedece.