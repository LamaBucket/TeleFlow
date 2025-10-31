namespace TeleFlow.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ValidateStatePropertyAttribute : Attribute
{
    public string PropertyDisplayName { get; internal init; }

    public ValidateStatePropertyAttribute(string propertyDisplayName)
    {
        PropertyDisplayName = propertyDisplayName;
    }
}