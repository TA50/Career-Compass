using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;
using CareerCompass.Infrastructure.Scenarios;
using CareerCompass.Infrastructure.Tags;
using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Infrastructure.Users;


[Table("Agents")]
internal class AgentTable : IAuditable
{
    public Guid Id { get; set; }

    #region Navigation

    public IdentityUser User { get; set; }
    public ICollection<ScenarioTable> Scenarios { get; set; } = [];
    public ICollection<TagTable> Tags { get; set; } = [];
    public ICollection<FieldTable> Fields { get; set; } = [];

    #endregion

    #region IAuditable

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}