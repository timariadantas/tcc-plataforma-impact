using ClientService.Domain;
using ClientService.DTO.Responses;

namespace ClientService.Mappers
{
    public static class ClientMapper
    {
        public static ClientResponseDto ToResponse (Client client)
        {
            return new ClientResponseDto
            {
                Id = client.Id,
                Name = client.Name,
                Surname = client.Surname,
                Email = client.Email,
                Birthdate = client.Birthdate,
                Active = client.Active

            };
        }
    }
}