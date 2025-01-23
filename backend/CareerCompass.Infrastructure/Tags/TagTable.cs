using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Scenarios;
using CareerCompass.Infrastructure.Users;

namespace CareerCompass.Infrastructure.Tags;

internal class TagTable : IAuditable
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    #region Navigation

    public required UserTable User { get; set; }
    public ICollection<ScenarioTable> Scenarios { get; set; } = [];

    #endregion


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}