using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;

namespace Hrm.Application.Lifecycle;

internal static class LifOffboardingMapper
{
    public static LifOffboardingDto ToDto(LifOffboardingSnapshot s)
    {
        DateOnly? nPlus3 = s.LastWorkingDayN is { } n
            ? LifOffboardingFacts.ComputeNPlus3(n)
            : null;
        var eligible = s.Status == LifOffboardingStatus.ConfirmedN && s.LastWorkingDayN.HasValue;
        return new LifOffboardingDto(
            s.Id,
            s.EmployeeId,
            s.EmployeeCode,
            s.Source,
            s.Status.ToString(),
            s.LastWorkingDayN,
            nPlus3,
            s.ResignationSignedDate,
            eligible,
            s.ConfirmedByIdpSubject,
            s.ConfirmedAtUtc,
            s.CreatedAtUtc,
            s.CreatedByIdpSubject,
            s.Note);
    }
}
