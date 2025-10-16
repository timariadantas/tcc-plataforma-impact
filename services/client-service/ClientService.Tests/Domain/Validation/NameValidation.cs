using ClientService.Domain;
using System;
using Xunit;

using ClientService.Domain.Validation;


namespace ClientService.Testes.Domain
{
    public class NameValidationTests
    {
        [Fact]
        public void ShouldThrow_WhenNameIsEmpty() //Deveria lançar quando o nome estiver vazio
        {
            var client = new Client { Name = "", Surname = "Dantas" };
            var validator = new NameValidation();

            var ex = Assert.Throws<ArgumentException>(() => validator.Validate(client));
            Assert.Equal("Nome é obrigatório", ex.Message);
        }

        [Fact]
        public void ShouldThrow_WhenSurnameIsEmpty() //Deveria lançar quando o sobrenome estiver vazio
        {
            //Arrange:
            var client = new Client { Name = "Maria", Surname = "" };
            var validator = new NameValidation();

            //Act & Assert:
            var ex = Assert.Throws<ArgumentException>(() => validator.Validate(client));
            Assert.Equal("Sobrenome é obrigatório.", ex.Message);
        }

        [Fact]
        public void ShouldPass_WhenNameAndSurnameAreValid()  //Deve passar quando nome e sobrenome são válidos
        {
            var client = new Client { Name = "Maria", Surname = "Dantas" };
            var validator = new NameValidation();

            validator.Validate(client); // não deve lançar exceção
        }
    }
}
