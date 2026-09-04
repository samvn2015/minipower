using Jarvis.Domain.Entities;

namespace Hrm.Domain.Probation.Entities;

/// <summary>Master thời lượng gia hạn TV — PRB-FR-006 (cấm số tháng tự do).</summary>
public class ProbationExtendDuration : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public int Months { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}
