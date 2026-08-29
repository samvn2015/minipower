using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees;

internal static class EmpEducationGuard
{
    public static async Task RequireActiveEducationLevelAsync(
        IEducationLevelReadRepository educationLevels,
        string? educationLevelCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(educationLevelCode))
            return;

        if (!await educationLevels.IsActiveAsync(educationLevelCode, cancellationToken).ConfigureAwait(false))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Trình độ học vấn không thuộc catalog hiệu lực (EMP-FR-017).");
        }
    }
}
