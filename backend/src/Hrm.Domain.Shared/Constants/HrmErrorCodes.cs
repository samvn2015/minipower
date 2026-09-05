using System.ComponentModel.DataAnnotations;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Domain.Shared.Constants;

public sealed class HrmErrorCodes : IErrorCode
{
    [Display(Description = "Unauthorized")]
    public const string Unauthorized = "40101";

    [Display(Description = "Forbidden")]
    public const string Forbidden = "40301";

    [Display(Description = "Not found")]
    public const string NotFound = "40401";

    [Display(Description = "Bad request")]
    public const string BadRequest = "40001";

    [Display(Description = "Conflict")]
    public const string Conflict = "40901";
}
