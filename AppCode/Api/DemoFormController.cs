#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

using AppCode.Extensions.GoogleRecaptchaV3;
using System.Threading.Tasks;
using ToSic.Sxc.WebApi;

// Simple API controller used by Docs-Recaptcha.cshtml
[AllowAnonymous]
public class DemoFormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  [SecureEndpoint]
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

    // Your custom logic goes here (e.g., save the form, send email, etc.)
    // Only reached when reCAPTCHA validation succeeded

    // Demo success response
    return Ok();
  }
}
