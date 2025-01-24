using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;

namespace CareerCompass.Infrastructure.Scenarios;

[Table("ScenarioFields")]
internal class ScenarioFieldTable : IAuditable
{
    public Guid Id { get; set; }
    
    public required string Value { get; set; }

    public Guid ScenarioId { get; set; }
    public Guid FieldId { get; set; }
    
    public  ScenarioTable Scenario { get; set; }

    
    public  FieldTable Field { get; set; }


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}