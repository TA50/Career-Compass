using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Persistence.Scenarios;
using CareerCompass.Infrastructure.Persistence.Users;


namespace CareerCompass.Infrastructure.Persistence.Tags;

[Table("Tags")]
internal class TagTable : IAuditable
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    #region Navigation

    public  AgentTable Agent { get; set; }
    
    public ICollection<ScenarioTable> Scenarios { get; set; } = [];

    #endregion


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}