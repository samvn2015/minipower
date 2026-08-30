using Hrm.Application.Leave.Commands;
using Hrm.Application.Tests.Employees;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Leave;

public sealed class ApproveLeaveRequestC1CommandHandlerTests
{
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid LmEmployeeId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid RequestId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    [Fact]
    public async Task HandleAsync_LmApproves_MovesToPendingC2()
    {
        var handler = new ApproveLeaveRequestC1CommandHandler(
            new FakeAccountRepo("local-lm", "MNV-HO"),
            new FakeEmployeeRepo(),
            new FakeLeaveRequestRepo());

        var result = await handler.HandleAsync(new ApproveLeaveRequestC1Command("local-lm", RequestId));

        Assert.Equal("PendingC2", result.Status);
    }

    [Fact]
    public async Task HandleAsync_NvSelfApprove_ThrowsForbidden()
    {
        var handler = new ApproveLeaveRequestC1CommandHandler(
            new FakeAccountRepo("local-dev", "MNV-DEV"),
            new FakeEmployeeRepo(),
            new FakeLeaveRequestRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ApproveLeaveRequestC1Command("local-dev", RequestId)));
    }

    private sealed class FakeAccountRepo(string sub, string employeeCode) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(),
                sub,
                sub,
                null,
                employeeCode,
                IdentityAccountStatus.Active,
                ["IAM-ROLE-NV"]));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeEmployeeRepo : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == EmployeeId)
            {
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(EmployeeId, lineManagerId: LmEmployeeId));
            }

            if (id == LmEmployeeId)
            {
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(LmEmployeeId, "MNV-HO", "Handover NV"));
            }

            return Task.FromResult<EmployeeSnapshot?>(null);
        }

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
        {
            if (employeeCode == "MNV-DEV")
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(EmployeeId, lineManagerId: LmEmployeeId));
            if (employeeCode == "MNV-HO")
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(LmEmployeeId, "MNV-HO", "Handover NV"));
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

    private sealed class FakeLeaveRequestRepo : ILeaveRequestRepository
    {
        public Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<LeaveRequestSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<LeaveRequestSnapshot?>(new LeaveRequestSnapshot(
                RequestId,
                EmployeeId,
                "LEV-ANNUAL",
                "Phép năm",
                new DateOnly(2026, 12, 1),
                new DateOnly(2026, 12, 1),
                LeaveDayPart.FullDay,
                1m,
                "Test",
                LmEmployeeId,
                LeaveRequestStatus.PendingC1,
                false,
                DateTime.UtcNow));

        public Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestSnapshot>>([]);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC1ByLineManagerIdAsync(
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC1Async(
            Guid id,
            string reviewedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> RejectC1Async(
            Guid id,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC2Async(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC2Async(
            Guid id,
            string reviewedByIdpSubject,
            bool deductsAnnualBalance,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> RejectC2Async(
            Guid id,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
    }
}
