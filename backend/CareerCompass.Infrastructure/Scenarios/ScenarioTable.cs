using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Tags;
using CareerCompass.Infrastructure.Users;

namespace CareerCompass.Infrastructure.Scenarios;

internal class ScenarioTable : IAuditable
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime? Date { get; set; }


    public ICollection<TagTable> Tags { get; set; } = [];
    public ICollection<ScenarioFieldTable> ScenarioFields { get; set; } = [];
    public required UserTable User { get; set; }


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}