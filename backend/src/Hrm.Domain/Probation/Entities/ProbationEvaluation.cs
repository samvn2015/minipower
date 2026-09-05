using Jarvis.Domain.Entities;

namespace Hrm.Domain.Probation.Entities;

public class ProbationOutcome : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}

public class ProbationCriterion : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}

/// <summary>Phiếu đánh giá TV — đề xuất LM / chốt HR (FR-009 · FR-017 audit).</summary>
public class ProbationEvaluation : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public DateOnly ProbationEndDate { get; set; }

    public ProbationEvaluationStatus Status { get; set; } = ProbationEvaluationStatus.Open;

    public string? ProposedOutcomeCode { get; set; }

    public string? ProposedByIdpSubject { get; set; }

    public DateTime? ProposedAtUtc { get; set; }

    public string? ProposedNote { get; set; }

    /// <summary>JSON scores [{criterionCode, comment}] — phiếu động FR-012.</summary>
    public string? CriteriaPayloadJson { get; set; }

    public string? DecidedOutcomeCode { get; set; }

    public string? DecidedByIdpSubject { get; set; }

    public DateTime? DecidedAtUtc { get; set; }

    public string? DecisionNote { get; set; }

    /// <summary>Mã thời lượng gia hạn từ master (FR-006) — chỉ khi EXTEND.</summary>
    public string? ExtendDurationCode { get; set; }
}
