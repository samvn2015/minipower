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

    public static readonly Guid OnPaperworkId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
    public static readonly Guid OnOrientationId = Guid.Parse("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2");
    public static readonly Guid OnBuddyId = Guid.Parse("a3a3a3a3-a3a3-a3a3-a3a3-a3a3a3a3a3a3");

    public static LifOnChecklistItem[] OnChecklistItems() =>
    [
        new()
        {
            Id = OnPaperworkId,
            Code = "ON-PAPERWORK",
            Name = "Hồ sơ / giấy tờ nhận việc",
            IsMust = true,
            IsActive = true,
            SortOrder = 1
        },
        new()
        {
            Id = OnOrientationId,
            Code = "ON-ORIENTATION",
            Name = "Orientation nội bộ",
            IsMust = true,
            IsActive = true,
            SortOrder = 2
        },
        new()
        {
            Id = OnBuddyId,
            Code = "ON-BUDDY",
            Name = "Gán buddy",
            IsMust = false,
            IsActive = true,
            SortOrder = 3
        }
    ];
}
