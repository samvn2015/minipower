using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

/// <summary>
/// Transaction trên DbContext scoped của context — mọi repository cùng scope dùng chung
/// connection nên <c>SaveChanges</c> của chúng đều nằm trong transaction này.
/// Nếu đã có transaction (lồng), chạy thẳng và để lớp ngoài commit.
/// </summary>
internal abstract class EfAtomicScope(DbContext db) : IAtomicScope
{
    public async Task<T> RunAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken cancellationToken = default)
    {
        if (db.Database.CurrentTransaction is not null)
            return await work(cancellationToken).ConfigureAwait(false);

        await using var transaction = await db.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        var result = await work(cancellationToken).ConfigureAwait(false);

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }
}

internal sealed class LevAtomicScope(LevDbContext db) : EfAtomicScope(db), ILevAtomicScope;

internal sealed class IamAtomicScope(IamDbContext db) : EfAtomicScope(db), IIamAtomicScope;
