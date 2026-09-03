using Hrm.Application.Timekeeping;
using Hrm.Application.Timekeeping.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class PreviewTimesheetImportCommandHandlerTests
{
    private static readonly Guid TemplateId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_WrongVersion_ThrowsBadRequest()
    {
        var handler = new PreviewTimesheetImportCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTemplateRepo(),
            new FakeEmployeeRepo(),
            new FakeImportRepo());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new PreviewTimesheetImportCommand(
                "local-dev",
                "2026-09",
                "TIM-WRONG",
                "x.csv",
                [new TimesheetImportRowValidator.RawImportRow(1, "MNV-DEV", 22, 0, 0, 0, null)])));
    }

    [Fact]
    public async Task HandleAsync_MissingEmployee_HasMustErrors()
    {
        var imports = new FakeImportRepo();
        var handler = new PreviewTimesheetImportCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTemplateRepo(),
            new FakeEmployeeRepo(),
            imports);

        var result = await handler.HandleAsync(new PreviewTimesheetImportCommand(
            "local-dev",
            "2026-09",
            "TIM-V1",
            "x.csv",
            [new TimesheetImportRowValidator.RawImportRow(1, "NO-SUCH", 22, null, null, null, null)]));

        Assert.True(result.HasMustErrors);
        Assert.Equal(1, result.ErrorRows);
    }

    [Fact]
    public async Task HandleAsync_LmCannotImport_ThrowsForbidden()
    {
        var handler = new PreviewTimesheetImportCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakeTemplateRepo(),
            new FakeEmployeeRepo(),
            new FakeImportRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new PreviewTimesheetImportCommand(
                "local-lm",
                "2026-09",
                "TIM-V1",
                null,
                [new TimesheetImportRowValidator.RawImportRow(1, "MNV-DEV", 22, null, null, null, null)])));
    }

    [Fact]
    public async Task Commit_WithMustErrors_ThrowsBadRequest()
    {
        var imports = new FakeImportRepo { BatchHasErrors = true };
        var handler = new CommitTimesheetImportCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CommitTimesheetImportCommand("local-dev", Guid.NewGuid())));
    }

    private sealed class FakeAccountRepo(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, null, IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeTemplateRepo : ITimesheetTemplateRepository
    {
        public Task<TimesheetTemplateVersionSnapshot?> FindActiveAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetTemplateVersionSnapshot?>(new TimesheetTemplateVersionSnapshot(
                TemplateId, "TIM-V1", "V1", TimesheetTemplateStatus.Active, DateTime.UtcNow, "seed",
                [new TimesheetTemplateColumnSnapshot(Guid.NewGuid(), "mnv", "MNV", 1, true, "EmployeeCode")]));

        public Task<TimesheetTemplateVersionSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => FindActiveAsync(cancellationToken);

        public Task<IReadOnlyList<TimesheetTemplateVersionSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TimesheetTemplateVersionSnapshot>>([]);

        public Task<Guid> CreateDraftAsync(TimesheetTemplateCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<bool> ExistsByVersionCodeAsync(string versionCode, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> PublishAsync(Guid id, string publishedByIdpSubject, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<int> CountActiveAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class FakeEmployeeRepo : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
        {
            if (employeeCode == "MNV-DEV")
            {
                return Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                    EmployeeId, "MNV-DEV", "Dev", null, null, null, null, null, null, null, null,
                    null, EmployeeStatus.Active));
            }

            return Task.FromResult<EmployeeSnapshot?>(null);
        }

        public Task<EmployeeSnapshot?> FindByEmailCtyAsync(
            string emailCty,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeUniqueField?>(null);
    }

    private sealed class FakeImportRepo : ITimesheetImportRepository
    {
        public bool BatchHasErrors { get; init; }

        public Task<Guid> CreatePreviewAsync(
            TimesheetImportBatchCreateModel model,
            CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            LastModel = model;
            LastId = id;
            return Task.FromResult(id);
        }

        public Guid LastId { get; private set; }
        public TimesheetImportBatchCreateModel? LastModel { get; private set; }

        public Task<TimesheetImportBatchSnapshot?> FindBatchByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (BatchHasErrors)
            {
                return Task.FromResult<TimesheetImportBatchSnapshot?>(new TimesheetImportBatchSnapshot(
                    id, "2026-09", TemplateId, "TIM-V1", TimesheetImportBatchStatus.Preview,
                    "local-dev", DateTime.UtcNow, null, 1, 1, true,
                    [new TimesheetImportRowSnapshot(
                        Guid.NewGuid(), 1, "X", null, null, null, null, null, null, false, "E", "err")]));
            }

            var model = LastModel!;
            return Task.FromResult<TimesheetImportBatchSnapshot?>(new TimesheetImportBatchSnapshot(
                LastId,
                model.PeriodYm,
                model.TemplateVersionId,
                model.TemplateVersionCode,
                TimesheetImportBatchStatus.Preview,
                model.UploadedByIdpSubject,
                DateTime.UtcNow,
                model.FileName,
                model.Rows.Count,
                model.Rows.Count(r => !r.IsOk),
                model.Rows.Any(r => !r.IsOk),
                model.Rows.Select(r => new TimesheetImportRowSnapshot(
                    Guid.NewGuid(), r.RowNumber, r.EmployeeCode, r.EmployeeId, r.WorkDays, r.Ot15, r.Ot20, r.Ot30,
                    r.OtUnclassified, r.IsOk, r.ErrorCode, r.ErrorMessage)).ToList()));
        }

        public Task<TimesheetPeriodSnapshot?> FindPeriodByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<TimesheetPeriodSnapshot>>([]);

        public Task<TimesheetPeriodSnapshot?> CommitAsync(
            Guid batchId,
            string committedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> ClosePeriodAsync(
            string periodYm,
            string closedByIdpSubject,
            IReadOnlyList<TimesheetLeaveMergeLine> leaveMerge,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> UnlockPeriodAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);
    }
}
