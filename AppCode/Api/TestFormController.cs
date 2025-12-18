#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

using AppCode.Extensions.GoogleRecaptchaV3.Recaptcha;
using System.Threading.Tasks;
using ToSic.Sys.Utils;

[AllowAnonymous]
public class TestFormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  public async Task<IActionResult> SubmitAsync([FromBody] DemoFormRequest request)
  {
    if (request == null || string.IsNullOrWhiteSpace(request.Message))
      return Json(new { ok = false, error = "message_missing" });

    if (string.IsNullOrWhiteSpace(request.Token))
      return Json(new { ok = false, error = "token_missing" });

    var remoteIp = System.Web.HttpContext.Current?.Request?.UserHostAddress;

    var validator = GetService<RecaptchaValidator>();
    var result = await validator.ValidateAsync(
      token: request.Token + "BB",
      remoteIp: remoteIp
    );
    if (!result.IsValid)
      return BadRequest(result.Error);

    return Json(new { ok = true });
  }
}

public class DemoFormRequest
{
  public string Token { get; set; }
  public string Message { get; set; }
}
