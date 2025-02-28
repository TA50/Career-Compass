using System.ComponentModel.DataAnnotations;

namespace CareerCompass.Api.Validation;

public class IsGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
            return true; // Null is considered valid, to validate for this use required

        if (value is string stringValue)
        {
            return Guid.TryParse(stringValue, out _); // Validate if it's a GUID
        }

        return false;
    }
}


