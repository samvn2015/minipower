using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Entities;

namespace Hrm.Infrastructure.Persistence.Probation;

internal static class PrbSeed
{
    public static readonly Guid OutcomePassId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1");
    public static readonly Guid OutcomeExtendId = Guid.Parse("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2");
    public static readonly Guid OutcomeFailId = Guid.Parse("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3");

    public static readonly Guid CritWorkId = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4");
    public static readonly Guid CritAttitudeId = Guid.Parse("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5");

    public static readonly Guid Ext1mId = Guid.Parse("d6d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6");
    public static readonly Guid Ext2mId = Guid.Parse("d7d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7");

    public static IEnumerable<object> Outcomes() =>
    [
        new ProbationOutcome
        {
            Id = OutcomePassId, Code = ProbationOutcomeCodes.Pass, Name = "Đạt", IsActive = true, SortOrder = 1
        },
        new ProbationOutcome
        {
            Id = OutcomeExtendId, Code = ProbationOutcomeCodes.Extend, Name = "Gia hạn", IsActive = true, SortOrder = 2
        },
        new ProbationOutcome
        {
            Id = OutcomeFailId, Code = ProbationOutcomeCodes.Fail, Name = "Không đạt", IsActive = true, SortOrder = 3
        }
    ];

    public static IEnumerable<object> Criteria() =>
    [
        new ProbationCriterion
        {
            Id = CritWorkId, Code = "CRIT-WORK", Name = "Kết quả công việc", IsActive = true, SortOrder = 1
        },
        new ProbationCriterion
        {
            Id = CritAttitudeId, Code = "CRIT-ATTITUDE", Name = "Thái độ / kỷ luật", IsActive = true, SortOrder = 2
        }
    ];

    public static IEnumerable<object> ExtendDurations() =>
    [
        new ProbationExtendDuration
        {
            Id = Ext1mId, Code = "EXT-1M", Name = "Gia hạn 1 tháng", Months = 1, IsActive = true, SortOrder = 1
        },
        new ProbationExtendDuration
        {
            Id = Ext2mId, Code = "EXT-2M", Name = "Gia hạn 2 tháng", Months = 2, IsActive = true, SortOrder = 2
        }
    ];
}
