using Hrm.Application.Identity.Queries;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;

namespace Hrm.Application.Tests.Identity;

public sealed class GetCurrentUserQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsRolesFromIamDb()
    {
        var handler = new GetCurrentUserQueryHandler(new FakeIdentityAccountReadRepository(
            new IdentityAccountSnapshot(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                "sub-1",
                "Hùng IAM",
                "hung@company.local",
                "MNV001",
                IdentityAccountStatus.Active,
                ["IAM-ROLE-NV", "IAM-ROLE-HR"])));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("sub-1", "Token Name", ["ignored-from-token"]));

        Assert.Equal("sub-1", result.Sub);
        Assert.Equal("Hùng IAM", result.Name);
        Assert.Equal(2, result.Roles.Count);
        Assert.Contains("IAM-ROLE-NV", result.Roles);
        Assert.Contains("IAM-ROLE-HR", result.Roles);
        Assert.Null(result.Note);
    }

    [Fact]
    public async Task HandleAsync_DisabledAccount_ReturnsEmptyRoles()
    {
        var handler = new GetCurrentUserQueryHandler(new FakeIdentityAccountReadRepository(
            new IdentityAccountSnapshot(
                Guid.NewGuid(),
                "sub-disabled",
                "Disabled User",
                null,
                null,
                IdentityAccountStatus.Disabled,
                ["IAM-ROLE-NV"])));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("sub-disabled", null, []));

        Assert.Empty(result.Roles);
        Assert.Contains("vô hiệu", result.Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_UnknownSubject_ReturnsEmptyRoles()
    {
        var handler = new GetCurrentUserQueryHandler(new FakeIdentityAccountReadRepository(null));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("unknown-sub", "X", ["Admin"]));

        Assert.Empty(result.Roles);
        Assert.Contains("IAM DB", result.Note, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleAsync_NullQuery_ThrowsArgumentNullException()
    {
        var handler = new GetCurrentUserQueryHandler(new FakeIdentityAccountReadRepository(null));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => handler.HandleAsync(null!));
    }

    private sealed class FakeIdentityAccountReadRepository(IdentityAccountSnapshot? snapshot)
        : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult(
                snapshot is not null && snapshot.IdpSubject == idpSubject ? snapshot : null);
    }
}
