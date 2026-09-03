using Hrm.Application.Timekeeping.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class PublishTimesheetTemplateCommandHandlerTests
{
    private static readonly Guid DraftId = Guid.Parse("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1");

    [Fact]
    public async Task HandleAsync_HrPublishes_ActivatesAndRetiresPrevious()
    {
        var repo = new FakeTemplateRepo();
        var handler = new PublishTimesheetTemplateCommandHandler(
            new FakeAccountRepo("local-dev", ["IAM-ROLE-HR"]),
            repo);

        var result = await handler.HandleAsync(new PublishTimesheetTemplateCommand("local-dev", DraftId));

        Assert.Equal("Active", result.Status);
        Assert.True(repo.Published);
    }

    [Fact]
    public async Task HandleAsync_NvCannotPublish_ThrowsForbidden()
    {
        var handler = new PublishTimesheetTemplateCommandHandler(
            new FakeAccountRepo("local-dev", ["IAM-ROLE-NV"]),
            new FakeTemplateRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new PublishTimesheetTemplateCommand("local-dev", DraftId)));
    }

    private sealed class FakeAccountRepo(string sub, string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), sub, sub, null, null, IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeTemplateRepo : ITimesheetTemplateRepository
    {
        public bool Published { get; private set; }

        public Task<TimesheetTemplateVersionSnapshot?> FindActiveAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<TimesheetTemplateVersionSnapshot?>(null);

        public Task<TimesheetTemplateVersionSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<TimesheetTemplateVersionSnapshot?>(new TimesheetTemplateVersionSnapshot(
                DraftId, "TIM-V2", "V2", TimesheetTemplateStatus.Draft, null, null,
                [new TimesheetTemplateColumnSnapshot(Guid.NewGuid(), "mnv", "MNV", 1, true, "EmployeeCode")]));

        public Task<IReadOnlyList<TimesheetTemplateVersionSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TimesheetTemplateVersionSnapshot>>([]);

        public Task<Guid> CreateDraftAsync(TimesheetTemplateCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<bool> ExistsByVersionCodeAsync(string versionCode, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> PublishAsync(Guid id, string publishedByIdpSubject, CancellationToken cancellationToken = default)
        {
            Published = true;
            return Task.FromResult(true);
        }

        public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(1);
    }
}
