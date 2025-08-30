using System;
using Xunit;
using ClientService.Domain;

namespace ClientService.Tests.Domain
{
    public class ClientFactoryTests
    {
        [Fact]
        public void ShouldCreateClientWithIdAndCreateAt()
        {
            var client = ClientFactory.Create("Maria", "Das Dores", "maria@example.com", new DateTime(2000, 1, 1));

            Assert.False(string.IsNullOrWhiteSpace(client.Id));
            Assert.Equal("Maria", client.Name);
            Assert.Equal("Das Dores", client.Surname);
            Assert.Equal("maria@example.com", client.Email);
            Assert.Equal(new DateTime(2000, 1, 1), client.Birthdate);
            Assert.True(client.CreatedAt <= DateTime.UtcNow);
        }
    }
}

// Anotações IPC
//ClientFactory garante consistência: todo cliente tem Id e CreateAt automáticos.
//Ulid.NewUlid().ToString() gera um identificador único melhor que GUID para ordenação por tempo.
//DateTime.UtcNow evita problemas de fuso horário ao criar a data de criação.