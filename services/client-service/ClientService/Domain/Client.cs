
using System;
using NUlid;


namespace ClientService.Domain
{



    public class Client
    {
        public string Id { get; set; } = NUlid.Ulid.NewUlid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public DateTime CreatedAt { get; set; } //// preenchido pelo banco
        public DateTime UpdatedAt { get; set; } // preenchido pelo banco
        public bool Active { get; set; } = true; //// default true no banco

    }

}


// Boa prática: o identificador (ULID/UUID) deve ser gerado na aplicação (Domain) e não no banco.
// O Domain cria o ID (ULID).O banco só armazena.
// DDD + Clean Architecture:
// O domínio manda, a infraestrutura obedece.