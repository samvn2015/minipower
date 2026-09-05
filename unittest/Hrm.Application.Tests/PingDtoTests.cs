using Hrm.Application.DTOs;

namespace Hrm.Application.Tests;

public class PingDtoTests
{
    [Fact]
    public void PingDto_HoldsStatusAndProduct()
    {
        var dto = new PingDto("ok", "Hrm");
        Assert.Equal("ok", dto.Status);
        Assert.Equal("Hrm", dto.Product);
    }
}
