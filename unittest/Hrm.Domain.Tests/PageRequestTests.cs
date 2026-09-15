using Hrm.Domain.Shared.Paging;

namespace Hrm.Domain.Tests;

/// <summary>
/// Code-review S1 — trần phân trang. Đầu vào sai của client được **kẹp**, không ném lỗi:
/// trả một trang hợp lệ hữu ích hơn trả 400 cho một tham số gõ nhầm.
/// </summary>
public sealed class PageRequestTests
{
    [Fact]
    public void KhongTruyenGi_DungMacDinh_CoTran()
    {
        var p = PageRequest.From(null, null);

        Assert.Equal(1, p.Page);
        Assert.Equal(PageRequest.DefaultSize, p.Size);
        Assert.Equal(0, p.Skip);
    }

    [Theory]
    [InlineData(999_999)]
    [InlineData(int.MaxValue)]
    public void SizeQuaLon_BiKepVeMaxSize(int size)
    {
        // Đây là chỗ chặn truy vấn không giới hạn — lý do S1 tồn tại.
        Assert.Equal(PageRequest.MaxSize, PageRequest.From(1, size).Size);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void PageKhongHopLe_VeTrang1(int page)
    {
        Assert.Equal(1, PageRequest.From(page, 10).Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SizeKhongHopLe_VeMacDinh(int size)
    {
        Assert.Equal(PageRequest.DefaultSize, PageRequest.From(1, size).Size);
    }

    [Fact]
    public void Skip_TinhTheoTrangVaKichThuoc()
    {
        Assert.Equal(50, PageRequest.From(3, 25).Skip);
    }
}
