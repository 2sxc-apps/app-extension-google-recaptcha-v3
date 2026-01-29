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
    if (string.IsNullOrWhiteSpace(request?.Message))
      return BadRequest("message_missing");

    // Validate reCAPTCHA token
    var validator = GetService<RecaptchaValidator>();
    var result = await validator.ValidateAsync(
      token: request.Token,
      remoteIp: Request.GetClientIp()
    );

    if (!result.Success)
      return BadRequest(result.Error);

    // Demo success response
    return Ok();
  }
}

// Request DTO for the demo form
public class DemoFormRequest
{
  public string Token { get; set; }
  public string Message { get; set; }
}
