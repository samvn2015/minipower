using Hrm.Application.Common;

namespace Hrm.Host.Services;

/// <summary>Đọc <c>Hrm:HostRole</c> — Active (mặc định) vs Standby.</summary>
public sealed class HostRoleGate(IConfiguration configuration) : IHostRoleGate
{
    public bool IsActiveHost()
    {
        var role = configuration["Hrm:HostRole"];
        if (string.IsNullOrWhiteSpace(role))
            return true;

        return !string.Equals(role.Trim(), "Standby", StringComparison.OrdinalIgnoreCase);
    }
}
