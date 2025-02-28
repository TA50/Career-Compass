using System.ComponentModel.DataAnnotations;

namespace CareerCompass.Api.Validation;

public class IsGuidListAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is List<string> list)
        {
            return list.All(item => Guid.TryParse(item, out _));
        }

        return false; // If it's not a List<string>, it's invalid
    }
}