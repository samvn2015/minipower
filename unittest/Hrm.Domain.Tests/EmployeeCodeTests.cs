using Hrm.Domain.ValueObjects;

namespace Hrm.Domain.Tests;

public class EmployeeCodeTests
{
    [Fact]
    public void Constructor_ValidCode_NormalizesUppercase()
    {
        var code = new EmployeeCode(" mnv-001 ");
        Assert.Equal("MNV-001", code.Value);
    }

    [Fact]
    public void Constructor_Blank_Throws()
    {
        Assert.Throws<ArgumentException>(() => new EmployeeCode("  "));
    }
}
