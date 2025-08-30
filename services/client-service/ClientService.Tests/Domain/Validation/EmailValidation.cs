using System;
using Xunit;
using ClientService.Domain;
using ClientService.Domain.Validation;

namespace ClientService.Tests.Domain
{
    public class EmailValidationTests
    {
        [Fact]
        public void ShouldThrow_WhenEmailIsEmpty()  // deve lançar quando o e-mail estiver vazio
        {
            //Arrangue
            var client = new Client { Email = "" };
            var validator = new EmailValidation();

            // Act & Assert:
            var ex = Assert.Throws<ArgumentException>(() => validator.Validate(client));
            Assert.Equal("Email inválido !", ex.Message);
        }

        [Fact]
        public void ShouldThrow_WhenEmailHasNoAtSymbol() //Deve lançar quando o e-mail não tiver símbolo
        {
            var client = new Client { Email = "maria.com" };
            var validator = new EmailValidation();

            var ex = Assert.Throws<ArgumentException>(() => validator.Validate(client));
            Assert.Equal("Email inválido !", ex.Message);
        }

        [Fact]
        public void ShouldPass_WhenEmailIsValid()  //Deve Passar Quando o E-mail for Válido
        {
            var client = new Client { Email = "maria@example.com" };
            var validator = new EmailValidation();

            validator.Validate(client); // não deve lançar exceção
        }
    }
}
//Testes unitários: verificam pequenas partes do código.
//Assert: é o juiz que decide se o teste passou ou falhou, comparando o resultado com o esperado.
//Cada [Fact] é um cenário independente.