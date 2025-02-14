namespace CareerCompass.Core.Common.Models;

public interface IAuditable
{
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
}