namespace Hrm.Domain.Shared.Paging;

/// <summary>
/// Code-review S1 — tham số phân trang có **chặn trên cứng**.
///
/// DOC-12 §3 quy định <c>page</c>/<c>size</c> nhưng code chưa hiện thực: mọi endpoint danh
/// sách đọc trọn bảng. Với `emp_employee`, `emp_audit_log`, `tim_timesheet_line` — các tập
/// tăng theo nhân sự × thời gian — đó là truy vấn không giới hạn.
///
/// <para><b>Mặc định chọn 200, không phải 50.</b> Trần tồn tại để chặn trường hợp bệnh lý,
/// không phải để đổi hành vi màn hình đang chạy: cắt mặc định xuống 50 sẽ khiến UI hiện tại
/// **âm thầm mất dòng** vì nó chưa có nút sang trang. Client muốn trang nhỏ thì truyền
/// <c>size</c> tường minh.</para>
/// </summary>
public sealed record PageRequest
{
    public const int DefaultSize = 200;

    public const int MaxSize = 500;

    private PageRequest(int page, int size)
    {
        Page = page;
        Size = size;
    }

    public int Page { get; }

    public int Size { get; }

    public int Skip => (Page - 1) * Size;

    /// <summary>
    /// Kẹp giá trị thay vì ném lỗi: <c>size=999999</c> hay <c>page=-1</c> là đầu vào sai của
    /// client, không phải sự cố hệ thống — trả trang hợp lệ hữu ích hơn trả 400.
    /// </summary>
    public static PageRequest From(int? page, int? size) =>
        new(
            page is null or < 1 ? 1 : page.Value,
            size switch
            {
                null => DefaultSize,
                < 1 => DefaultSize,
                > MaxSize => MaxSize,
                _ => size.Value,
            });
}
