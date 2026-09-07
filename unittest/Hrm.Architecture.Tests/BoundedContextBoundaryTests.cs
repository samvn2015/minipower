using System.Reflection;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Architecture.Tests;

/// <summary>
/// ADR-011 W2 — cưỡng chế ranh giới bounded context bằng **cơ chế**, không bằng quy ước.
///
/// W1 dựng hàng rào ở tầng database (schema + role). Bộ test này canh tầng code: nếu ai đó
/// thêm một DbSet hoặc một repository chạm sang context khác, test đỏ **trước** khi lên Prod
/// và phát hiện bằng lỗi 42501 lúc chạy thật.
///
/// Ngoại lệ **được phép** khai báo tường minh dưới đây — mỗi cái có trace tới quyết định:
///   ①a  EmpAuditLog (schema shared) — mọi context ghi audit của mình (DEC-ARC-024)
///   ②a  LEV đọc closure `emp` — golden record, DOC-11 §4 (DEC-ARC-019)
/// </summary>
public sealed class BoundedContextBoundaryTests
{
    /// <summary>Tiền tố namespace entity theo bounded context.</summary>
    private static readonly Dictionary<Type, string> ContextOwner = new()
    {
        [typeof(IamDbContext)] = "Hrm.Domain.Identity",
        [typeof(EmpDbContext)] = "Hrm.Domain.Employees",
        [typeof(LevDbContext)] = "Hrm.Domain.Leave",
        [typeof(TimDbContext)] = "Hrm.Domain.Timekeeping",
        [typeof(PayDbContext)] = "Hrm.Domain.Payroll",
        [typeof(PrbDbContext)] = "Hrm.Domain.Probation",
        [typeof(LifDbContext)] = "Hrm.Domain.Lifecycle",
    };

    /// <summary>
    /// Entity của context khác mà một context được phép map. Thêm dòng vào đây là hành vi
    /// **có ý thức** — phải kèm quyết định, không phải sửa cho test xanh.
    /// </summary>
    private static readonly Dictionary<Type, string[]> AllowedForeignEntities = new()
    {
        // ①a — audit dùng chung ở schema `shared`, ghi bằng context của chính module.
        [typeof(EmpDbContext)] = [],
        [typeof(TimDbContext)] = ["EmpAuditLog"],
        [typeof(PayDbContext)] = ["EmpAuditLog"],
        [typeof(PrbDbContext)] = ["EmpAuditLog"],
        [typeof(LifDbContext)] = ["EmpAuditLog"],
        [typeof(IamDbContext)] = [],

        // ②a — LEV JOIN sang emp để lọc hàng đợi C1/C2 theo line manager.
        // Map Employee kéo theo cả closure navigation → phải liệt kê đủ.
        // Đây là món nợ W3 phải trả bằng API, xem ADR-011.
        [typeof(LevDbContext)] =
        [
            "Employee", "EmployeeContract", "EducationLevel",
            "OrgUnit", "SeniorityRule", "LineManagerChangeRequest",
        ],
    };

    public static TheoryData<Type> Contexts()
    {
        var data = new TheoryData<Type>();
        foreach (var t in ContextOwner.Keys)
            data.Add(t);
        return data;
    }

    [Theory]
    [MemberData(nameof(Contexts))]
    public void DbContext_chi_map_entity_cua_context_minh_hoac_ngoai_le_da_khai(Type contextType)
    {
        var owner = ContextOwner[contextType];
        var allowed = AllowedForeignEntities[contextType];

        var foreign = contextType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .Where(e => e.Namespace is not null && !e.Namespace.StartsWith(owner, StringComparison.Ordinal))
            .Select(e => e.Name)
            .Where(name => !allowed.Contains(name))
            .OrderBy(name => name)
            .ToArray();

        Assert.True(
            foreign.Length == 0,
            $"{contextType.Name} map entity ngoài bounded context mà chưa khai ngoại lệ: "
            + $"{string.Join(", ", foreign)}. Thêm vào AllowedForeignEntities kèm quyết định, "
            + "hoặc chuyển sang gọi qua interface của context sở hữu.");
    }

    [Fact]
    public void AppDbContext_khong_con_duoc_repository_nao_dung()
    {
        // W1c: AppDbContext chỉ còn một vai — migration owner chạy bằng hrm_migrator (③ii).
        // Repository nào còn nhận nó là đang đi vòng qua hàng rào role.
        var offenders = typeof(AppDbContext).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.Namespace?.Contains(".Repositories", StringComparison.Ordinal) == true)
            .Where(t => t.GetConstructors()
                .Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(AppDbContext))))
            .Select(t => t.Name)
            .OrderBy(name => name)
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            $"Repository còn inject AppDbContext: {string.Join(", ", offenders)}. "
            + "Dùng DbContext của bounded context tương ứng.");
    }

    [Fact]
    public void Moi_bounded_context_deu_co_DbContext_rieng()
    {
        // Chặn hồi quy kiểu "thêm module mới nhưng nhét entity vào AppDbContext".
        Assert.Equal(7, ContextOwner.Count);
        foreach (var (contextType, owner) in ContextOwner)
        {
            var own = contextType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .Count(e => e.Namespace?.StartsWith(owner, StringComparison.Ordinal) == true);

            Assert.True(own > 0, $"{contextType.Name} không map entity nào của {owner}.");
        }
    }
}
