using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Persistence.Scenarios;
using CareerCompass.Infrastructure.Persistence.Users;


namespace CareerCompass.Infrastructure.Persistence.Fields;

[Table("Fields")]
internal class FieldTable : IAuditable
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    #region Navigations

    public ICollection<ScenarioFieldTable> ScenarioFields { get; set; } = [];
    public required AgentTable Agent { get; set; }

    #endregion

    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}