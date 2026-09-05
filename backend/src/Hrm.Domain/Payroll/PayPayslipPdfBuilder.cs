using System.Text;

namespace Hrm.Domain.Payroll;

/// <summary>PDF phiếu tối giản (không phụ thuộc lib) — PAY-FR-012.</summary>
public static class PayPayslipPdfBuilder
{
    public static byte[] Build(
        string periodYm,
        string employeeCode,
        decimal nTinh,
        decimal timeWageFactor,
        decimal contractAllowance,
        decimal monthlyAllowance,
        decimal bhAmount,
        decimal tncnAmount,
        decimal netPay)
    {
        var lines = new[]
        {
            $"HRM Payslip {periodYm}",
            $"Employee: {employeeCode}",
            $"N_tinh: {nTinh}",
            $"TV factor: {timeWageFactor}",
            $"PC HD: {contractAllowance}",
            $"PC thang: {monthlyAllowance}",
            $"BH: {bhAmount}",
            $"TNCN: {tncnAmount}",
            $"Thuc linh: {netPay}"
        };

        var content = new StringBuilder();
        content.Append("BT /F1 11 Tf 50 740 Td 14 TL\n");
        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
                content.Append("T*\n");
            content.Append('(').Append(Escape(lines[i])).Append(") Tj\n");
        }

        content.Append("ET");
        var stream = content.ToString();

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        var offsets = new List<int> { 0 };

        void WriteObj(string body)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(sb.ToString()));
            var n = offsets.Count - 1;
            sb.Append(n).Append(" 0 obj\n").Append(body).Append("\nendobj\n");
        }

        WriteObj("<< /Type /Catalog /Pages 2 0 R >>");
        WriteObj("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        WriteObj(
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>");
        WriteObj($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream");
        WriteObj("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

        var xref = Encoding.ASCII.GetByteCount(sb.ToString());
        sb.Append("xref\n0 ").Append(offsets.Count).Append('\n');
        sb.Append("0000000000 65535 f \n");
        for (var i = 1; i < offsets.Count; i++)
            sb.Append(offsets[i].ToString("D10")).Append(" 00000 n \n");
        sb.Append("trailer<< /Size ").Append(offsets.Count).Append(" /Root 1 0 R >>\n");
        sb.Append("startxref\n").Append(xref).Append("\n%%EOF\n");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private static string Escape(string text) =>
        text.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
}
