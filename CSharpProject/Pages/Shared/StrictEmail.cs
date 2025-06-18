using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class StrictEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;
        if (string.IsNullOrEmpty(email))
            return ValidationResult.Success;

        // Regex enforces a proper domain like "example.com"
        string pattern = @"^[^@\s]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(email, pattern))
            return new ValidationResult("Please enter a valid email address.");

        return ValidationResult.Success;
    }
}
