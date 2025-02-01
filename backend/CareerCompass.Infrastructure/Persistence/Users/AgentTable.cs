using System.ComponentModel.DataAnnotations.Schema;
using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Persistence.Fields;
using CareerCompass.Infrastructure.Persistence.Scenarios;
using CareerCompass.Infrastructure.Persistence.Tags;
using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Infrastructure.Persistence.Users;

[Table("Agents")]
internal class AgentTable : IAuditable
{
    public Guid Id { get; set; }
    public string UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }


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