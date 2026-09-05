using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>TIM-FR-010 / TIM-TC-010 — cấm protocol máy chấm công.</summary>
[ApiController]
[Route("v1/tim/devices")]
[Authorize]
public sealed class TimekeepingDeviceController : ControllerBase
{
    [HttpPost("{**catchAll}")]
    [HttpGet]
    [HttpGet("{**catchAll}")]
    public IActionResult RejectDeviceTraffic() =>
        Problem(
            statusCode: StatusCodes.Status405MethodNotAllowed,
            title: "Method Not Allowed",
            detail: "TIM-TC-010 / cấm máy CC — chỉ nhận file Excel/CSV đúng mẫu (TIM-FR-010).");
}
