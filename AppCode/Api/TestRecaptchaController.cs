#if NETCOREAPP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

using System.Threading.Tasks;
using ToSic.Sxc.WebApi;
using AppCode.Extensions.GoogleRecaptchaV3;

[AllowAnonymous]
public class TestRecaptchaController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  [SecureEndpoint]
  public async Task<IActionResult> Verify([FromBody] RecaptchaRequest request)
  {
    if (string.IsNullOrWhiteSpace(request?.Token))
      return BadRequest("token_missing");

    var result = await GetService<RecaptchaValidator>().ValidateAsync(
      token: request.Token,
      remoteIp: Request.GetClientIp(),
      minimumScore: request.MinimumScore > 0 ? request.MinimumScore : -1,
      expectedHostname: Request.GetHost());

    if (!result.Success)
      return BadRequest(result.Error);

    return Json(result);
  }
}

public class RecaptchaRequest
{
  public string Token { get; set; }
  public double MinimumScore { get; set; } = 0;
}