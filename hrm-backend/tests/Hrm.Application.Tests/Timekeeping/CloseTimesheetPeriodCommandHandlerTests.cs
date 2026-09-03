using Hrm.Application.Timekeeping.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class CloseTimesheetPeriodCommandHandlerTests
{
    private static readonly Guid PeriodId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_DraftClean_Closes()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(otUnclassified: 0, ot15: 2)]));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports);

        var result = await handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11"));

        Assert.Equal("Closed", result.Status);
        Assert.Equal("2026-11", result.PeriodYm);
        Assert.True(imports.CloseCalled);
    }

    [Fact]
    public async Task HandleAsync_OtUnclassified_ThrowsBadRequest()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(otUnclassified: 3, ot15: 0)]));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11")));
        Assert.False(imports.CloseCalled);
    }

    [Fact]
    public async Task HandleAsync_LmCannotClose_ThrowsForbidden()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                0,
                []));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            imports);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new CloseTimesheetPeriodCommand("local-lm", "2026-11")));
    }

    private static TimesheetLineSnapshot Line(decimal otUnclassified, decimal ot15) =>
        new(Guid.NewGuid(), EmployeeId, "MNV-DEV", 22, ot15, 0, 0, otUnclassified);

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

    private sealed class FakeImportRepo(TimesheetPeriodSnapshot period) : ITimesheetImportRepository
    {
        public bool CloseCalled { get; private set; }

        public Task<Guid> CreatePreviewAsync(
            TimesheetImportBatchCreateModel model,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Guid.NewGuid());

        public Task<TimesheetImportBatchSnapshot?> FindBatchByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetImportBatchSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> FindPeriodByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(
                periodYm == period.PeriodYm ? period : null);

        public Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<TimesheetPeriodSnapshot>>([period]);

        public Task<TimesheetPeriodSnapshot?> CommitAsync(
            Guid batchId,
            string committedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> ClosePeriodAsync(
            string periodYm,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default)
        {
            CloseCalled = true;
            return Task.FromResult<TimesheetPeriodSnapshot?>(period with
            {
                Status = TimesheetPeriodStatus.Closed
            });
        }
    }
}
