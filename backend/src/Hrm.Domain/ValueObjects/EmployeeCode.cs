namespace Hrm.Domain.ValueObjects;

public sealed class EmployeeCode
{
    public string Value { get; }

    public EmployeeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Employee code is required.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public override string ToString() => Value;
}
