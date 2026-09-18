using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;

namespace Hrm.Application.Tests.Common;

/// <summary>
/// Scope giả: chạy thẳng, đếm số lần vào/ra. Không mô phỏng rollback DB — cái đó là việc của
/// EfAtomicScope + PostgreSQL, kiểm bằng smoke; ở đây chỉ chứng minh handler ĐI QUA scope.
/// </summary>
public sealed class FakeAtomicScope : ILevAtomicScope, IIamAtomicScope
{
    public int Entered { get; private set; }

    public int Completed { get; private set; }

    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken cancellationToken = default)
    {
        Entered++;
        var result = await work(cancellationToken);
        Completed++;
        return result;
    }
}
