using Hrm.Domain.Lifecycle.Entities;

namespace Hrm.Infrastructure.Persistence.Lifecycle;

internal static class LifSeed
{
    public static readonly Guid MustLaptopId = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1");
    public static readonly Guid MustBadgeId = Guid.Parse("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2");
    public static readonly Guid MustHandoverId = Guid.Parse("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3");
    public static readonly Guid OptInterviewId = Guid.Parse("e4e4e4e4-e4e4-e4e4-e4e4-e4e4e4e4e4e4");

    public static LifOffChecklistItem[] OffChecklistItems() =>
    [
        new()
        {
            Id = MustLaptopId,
            Code = "OFF-RETURN-LAPTOP",
            Name = "Thu hồi laptop / thiết bị",
            IsMust = true,
            IsActive = true,
            SortOrder = 1
        },
        new()
        {
            Id = MustBadgeId,
            Code = "OFF-RETURN-BADGE",
            Name = "Thu hồi thẻ ra vào",
            IsMust = true,
            IsActive = true,
            SortOrder = 2
        },
        new()
        {
            Id = MustHandoverId,
            Code = "OFF-HANDOVER",
            Name = "Bàn giao công việc / tài liệu",
            IsMust = true,
            IsActive = true,
            SortOrder = 3
        },
        new()
        {
            Id = OptInterviewId,
            Code = "OFF-EXIT-INTERVIEW",
            Name = "Phỏng vấn nghỉ việc",
            IsMust = false,
            IsActive = true,
            SortOrder = 4
        }
    ];
}
