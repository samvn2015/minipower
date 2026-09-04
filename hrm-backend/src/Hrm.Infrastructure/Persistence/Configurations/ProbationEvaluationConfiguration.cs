using Hrm.Domain.Probation.Entities;
using Hrm.Infrastructure.Persistence.Probation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class ProbationOutcomeConfiguration : IEntityTypeConfiguration<ProbationOutcome>
{
    public void Configure(EntityTypeBuilder<ProbationOutcome> builder)
    {
        builder.ToTable("prb_outcome");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(PrbSeed.Outcomes().Cast<ProbationOutcome>());
    }
}

internal sealed class ProbationCriterionConfiguration : IEntityTypeConfiguration<ProbationCriterion>
{
    public void Configure(EntityTypeBuilder<ProbationCriterion> builder)
    {
        builder.ToTable("prb_criterion");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(PrbSeed.Criteria().Cast<ProbationCriterion>());
    }
}

internal sealed class ProbationExtendDurationConfiguration : IEntityTypeConfiguration<ProbationExtendDuration>
{
    public void Configure(EntityTypeBuilder<ProbationExtendDuration> builder)
    {
        builder.ToTable("prb_extend_duration");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(PrbSeed.ExtendDurations().Cast<ProbationExtendDuration>());
    }
}

internal sealed class ProbationEvaluationConfiguration : IEntityTypeConfiguration<ProbationEvaluation>
{
    public void Configure(EntityTypeBuilder<ProbationEvaluation> builder)
    {
        builder.ToTable("prb_evaluation");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.ProposedOutcomeCode).HasMaxLength(32);
        builder.Property(x => x.ProposedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.ProposedNote).HasMaxLength(2000);
        builder.Property(x => x.CriteriaPayloadJson).HasMaxLength(8000);
        builder.Property(x => x.DecidedOutcomeCode).HasMaxLength(32);
        builder.Property(x => x.DecidedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.DecisionNote).HasMaxLength(2000);
        builder.Property(x => x.ExtendDurationCode).HasMaxLength(32);
        builder.HasIndex(x => new { x.EmployeeId, x.Status });
        builder.HasIndex(x => x.EmployeeCode);
    }
}
