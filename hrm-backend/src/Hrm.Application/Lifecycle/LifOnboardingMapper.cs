using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Lifecycle.Repositories;

namespace Hrm.Application.Lifecycle;

internal static class LifOnboardingMapper
{
    public static LifOnboardingDto ToDto(LifOnboardingSnapshot s)
    {
        var all = s.EmailCtyProvisioned && s.GitProvisioned && s.CrmSpProvisioned && s.ChatProvisioned;
        return new LifOnboardingDto(
            s.Id,
            s.EmployeeId,
            s.EmployeeCode,
            s.Status.ToString(),
            s.CreatedAtUtc,
            s.CreatedByIdpSubject,
            s.Note,
            s.EmailCtyProvisioned,
            s.GitProvisioned,
            s.CrmSpProvisioned,
            s.ChatProvisioned,
            s.EmailCtyProvisionedAtUtc,
            s.GitProvisionedAtUtc,
            s.CrmSpProvisionedAtUtc,
            s.ChatProvisionedAtUtc,
            all,
            s.ClosedByIdpSubject,
            s.ClosedAtUtc);
    }
}
