namespace CurrencyService.Domain;

public class Quote
{
    public string Code { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
