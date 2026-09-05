namespace Hrm.Application.Common;

/// <summary>HA Active/Standby — job chỉ chạy trên host Active (PRB-TC-HA-001 · LIF-TC-HA-001 / ADR-003).</summary>
public interface IHostRoleGate
{
    bool IsActiveHost();
}
