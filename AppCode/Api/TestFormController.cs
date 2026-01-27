#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

using AppCode.Extensions.GoogleRecaptchaV3;
using System.Threading.Tasks;

// Demo API controller to test reCAPTCHA v3 form submissions
// Used by the frontend test form to verify token + message
[AllowAnonymous]
public class TestFormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  public async Task<IActionResult> SubmitAsync([FromBody] DemoFormRequest request)
  {
    // Basic input validation
    if (request == null || string.IsNullOrWhiteSpace(request.Message))
      return Json(new { ok = false, error = "message_missing" });

    // Forward client IP to Google (optional, improves validation)
    var remoteIp = System.Web.HttpContext.Current?.Request?.UserHostAddress;

    // Validate reCAPTCHA token
    var validator = GetService<RecaptchaValidator>();
    var result = await validator.ValidateAsync(
      token: request.Token,
      remoteIp: remoteIp
    );

    if (!result.Success)
      return BadRequest(result.Error);

    // Demo success response
    return Json(new { ok = true });
  }
}

// Request DTO for the demo form
public class DemoFormRequest
{
  public string Token { get; set; }
  public string Message { get; set; }
}
