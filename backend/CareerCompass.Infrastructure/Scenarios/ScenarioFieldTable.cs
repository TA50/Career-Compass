using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;

namespace CareerCompass.Infrastructure.Scenarios;

internal class ScenarioFieldTable : IAuditable
{
    public required string Value { get; set; }

    public Guid ScenarioId { get; set; }
    public Guid FieldId { get; set; }
    
    public required ScenarioTable Scenario { get; set; }

    
    public required FieldTable Field { get; set; }


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}