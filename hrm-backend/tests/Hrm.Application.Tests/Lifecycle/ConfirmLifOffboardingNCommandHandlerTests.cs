using Hrm.Application.Lifecycle.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Lifecycle;

public sealed class ConfirmLifOffboardingNCommandHandlerTests
{
    private static readonly Guid CaseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");

    [Fact]
    public async Task Confirm_Hr_SetsNAndNPlus3Eligible()
    {
        var repo = new FakeRepo(ResignationSignedDate: new DateOnly(2026, 8, 1));
        var handler = new ConfirmLifOffboardingNCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            repo);

        var dto = await handler.HandleAsync(
            new ConfirmLifOffboardingNCommand("local-dev", CaseId, new DateOnly(2026, 9, 30)));

        Assert.Equal("ConfirmedN", dto.Status);
        Assert.Equal(new DateOnly(2026, 9, 30), dto.LastWorkingDayN);
        Assert.Equal(new DateOnly(2026, 10, 3), dto.NPlus3Expected);
        Assert.True(dto.JobNPlus3Eligible);
        Assert.Equal("local-dev", dto.ConfirmedByIdpSubject);
    }

    [Fact]
    public async Task Confirm_NEqualsSignedDate_BadRequest()
    {
        var signed = new DateOnly(2026, 9, 15);
        var handler = new ConfirmLifOffboardingNCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeRepo(ResignationSignedDate: signed));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new ConfirmLifOffboardingNCommand("local-dev", CaseId, signed)));
    }

    [Fact]
    public async Task Confirm_Nv_Forbidden()
    {
        var handler = new ConfirmLifOffboardingNCommandHandler(
            new FakeAccounts(["IAM-ROLE-NV"], "MNV-DEV"),
            new FakeRepo(null));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(
                new ConfirmLifOffboardingNCommand("local-nv", CaseId, new DateOnly(2026, 9, 30))));
    }

    [Fact]
    public void ComputeNPlus3_CalendarDays()
    {
        Assert.Equal(
            new DateOnly(2026, 10, 3),
            LifOffboardingFacts.ComputeNPlus3(new DateOnly(2026, 9, 30)));
    }

    private sealed class FakeAccounts(string[] roles, string? employeeCode) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, employeeCode,
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeRepo(DateOnly? ResignationSignedDate) : ILifOffboardingRepository
    {
        public Task<LifOffboardingSnapshot> CreateAsync(
            LifOffboardingCreateModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<LifOffboardingSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LifOffboardingSnapshot?>(new LifOffboardingSnapshot(
                CaseId, EmpId, "MNV-X", "HR-MANUAL", LifOffboardingStatus.Open,
                null, ResignationSignedDate, null, null, DateTime.UtcNow, "hr", null));

        public Task<LifOffboardingSnapshot> ConfirmNAsync(
            Guid id,
            DateOnly lastWorkingDayN,
            string confirmedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new LifOffboardingSnapshot(
                id, EmpId, "MNV-X", "HR-MANUAL", LifOffboardingStatus.ConfirmedN,
                lastWorkingDayN, ResignationSignedDate, confirmedByIdpSubject, DateTime.UtcNow,
                DateTime.UtcNow, "hr", null));

        public Task<LifOffboardingSnapshot> CloseAsync(
            Guid id,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
