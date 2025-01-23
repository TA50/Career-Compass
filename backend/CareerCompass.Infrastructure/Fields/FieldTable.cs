using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Scenarios;
using CareerCompass.Infrastructure.Users;

namespace CareerCompass.Infrastructure.Fields;

internal class FieldTable : IAuditable
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    #region Navigations

    public ICollection<ScenarioFieldTable> ScenarioFields { get; set; } = [];
    public required UserTable User { get; set; }

    #endregion

    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}