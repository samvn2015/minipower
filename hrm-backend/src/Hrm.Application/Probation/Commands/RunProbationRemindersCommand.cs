using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Commands;

public sealed record RunProbationRemindersCommand(
    string ActorIdpSubject,
    DateOnly? AsOfDate) : ICommand;

public sealed record ProbationReminderRunResult(
    DateOnly AsOfDate,
    int T15Created,
    int T7Created,
    int SkippedIncompleteMilestone,
    int SkippedAlreadyExists);

public sealed class RunProbationRemindersCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IProbationReminderRepository reminders)
    : IAsyncCommandHandler<RunProbationRemindersCommand, ProbationReminderRunResult>
{
    public const string ChannelInAppAndEmail = "hrm-inapp+email";

    public async Task<ProbationReminderRunResult> HandleAsync(
        RunProbationRemindersCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor);

        var asOf = request.AsOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var all = await employees.ListAsync(cancellationToken);
        var byId = all.ToDictionary(e => e.Id);

        var toCreate = new List<ProbationReminderCreateModel>();
        var skippedIncomplete = 0;
        var skippedExists = 0;
        var t15 = 0;
        var t7 = 0;

        foreach (var emp in all.Where(e => e.Status == EmployeeStatus.Active))
        {
            if (!ProbationContractFacts.IsActiveProbationContract(emp.Contract))
                continue;

            var (_, end, complete) = ProbationContractFacts.ReadMilestones(emp.Contract);
            if (!complete || end is null)
            {
                skippedIncomplete++;
                continue;
            }

            var kt = end.Value;
            var t15Due = ProbationContractFacts.ComputeT15Date(kt)!.Value;
            var t7Due = ProbationContractFacts.ComputeT7Date(kt)!.Value;

            if (asOf == t15Due)
            {
                if (await reminders.ExistsAsync(emp.Id, ProbationReminderKind.T15, kt, cancellationToken))
                {
                    skippedExists++;
                }
                else
                {
                    var email = ResolveEmails(emp, byId, preferLm: true);
                    toCreate.Add(new ProbationReminderCreateModel(
                        ProbationReminderKind.T15,
                        emp.Id,
                        emp.EmployeeCode,
                        kt,
                        t15Due,
                        asOf,
                        emp.LineManagerEmployeeId,
                        ResolveCode(emp.LineManagerEmployeeId, byId),
                        $"T-15: NV {emp.EmployeeCode} kết thúc TV {kt:yyyy-MM-dd} (còn 15 ngày lịch).",
                        email,
                        ChannelInAppAndEmail,
                        request.ActorIdpSubject));
                    t15++;
                }
            }

            if (asOf == t7Due)
            {
                if (await reminders.ExistsAsync(emp.Id, ProbationReminderKind.T7, kt, cancellationToken))
                {
                    skippedExists++;
                }
                else
                {
                    Guid? assigneeId = emp.LineManagerEmployeeId;
                    string? assigneeCode = ResolveCode(assigneeId, byId);
                    // FR-014: không LM → gán HR (null assignee = HR pool)
                    if (assigneeId is null)
                    {
                        assigneeCode = null;
                    }

                    var email = ResolveEmails(emp, byId, preferLm: true);
                    toCreate.Add(new ProbationReminderCreateModel(
                        ProbationReminderKind.T7,
                        emp.Id,
                        emp.EmployeeCode,
                        kt,
                        t7Due,
                        asOf,
                        assigneeId,
                        assigneeCode,
                        assigneeId is null
                            ? $"T-7: Task đánh giá {emp.EmployeeCode} (không có LM → HR)."
                            : $"T-7: Task đánh giá {emp.EmployeeCode} cho LM {assigneeCode}.",
                        email,
                        ChannelInAppAndEmail,
                        request.ActorIdpSubject));
                    t7++;
                }
            }
        }

        if (toCreate.Count > 0)
            await reminders.AddManyAsync(toCreate, cancellationToken);

        return new ProbationReminderRunResult(asOf, t15, t7, skippedIncomplete, skippedExists);
    }

    private static string? ResolveCode(Guid? id, IReadOnlyDictionary<Guid, EmployeeSnapshot> byId) =>
        id is { } g && byId.TryGetValue(g, out var e) ? e.EmployeeCode : null;

    /// <summary>Email nội bộ NV (+ LM nếu có). Không CRM sales (FR-010).</summary>
    private static string ResolveEmails(
        EmployeeSnapshot emp,
        IReadOnlyDictionary<Guid, EmployeeSnapshot> byId,
        bool preferLm)
    {
        var addresses = new List<string>();
        if (!string.IsNullOrWhiteSpace(emp.EmailCty))
            addresses.Add(emp.EmailCty.Trim());

        if (preferLm && emp.LineManagerEmployeeId is { } lmId
            && byId.TryGetValue(lmId, out var lm)
            && !string.IsNullOrWhiteSpace(lm.EmailCty))
        {
            var lmEmail = lm.EmailCty.Trim();
            if (!addresses.Contains(lmEmail, StringComparer.OrdinalIgnoreCase))
                addresses.Add(lmEmail);
        }

        return addresses.Count == 0 ? "noreply@company.local" : string.Join(";", addresses);
    }
}
