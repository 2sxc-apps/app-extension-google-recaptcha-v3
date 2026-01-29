using AppCode.Extensions.GoogleRecaptchaV3;
using System.Threading.Tasks;
using ToSic.Sxc.WebApi;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

/// <summary>
/// Demo API controller to test reCAPTCHA v3 form submissions
/// Used by the frontend test form to verify token + message
/// </summary>
[AllowAnonymous]
public class DocsFormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  [SecureEndpoint]
  public async Task<IActionResult> SubmitAsync([FromBody] DemoFormRequest request)
  {
    // Basic input validation
    if (string.IsNullOrWhiteSpace(request?.Message))
      return BadRequest("Message-missing");

    // Validate reCAPTCHA token - include Client IP as it improves validation accuracy
    var result = await GetService<RecaptchaValidator>()
      .ValidateAsync(request.Token, remoteIp: Request.GetClientIp());

    if (!result.Success)
      return BadRequest(result.Error);

    // Your custom logic goes here (e.g., save the form, send email, etc.)
    // Only reached when reCAPTCHA validation succeeded

    // Demo success response
    return Ok();
  }
}
