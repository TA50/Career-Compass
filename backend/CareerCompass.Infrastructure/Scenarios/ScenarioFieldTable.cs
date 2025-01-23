using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;

namespace CareerCompass.Infrastructure.Scenarios;

internal class ScenarioFieldTable : IAuditable
{
    public required string Value { get; set; }
    
    [Key, Column(Order = 0)] 
    public required ScenarioTable Scenario { get; set; }

    [Key, Column(Order = 1)]
    public required FieldTable Field { get; set; }


    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}