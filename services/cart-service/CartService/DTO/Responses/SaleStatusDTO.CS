namespace CartService.DTO.Responses
{
    public class SaleStatusDTO
    {
        public string Name {get;set;}= string.Empty;
    }
}

// Nunca use SaleStatusDTO.Started
//Isso NÃO é enum.