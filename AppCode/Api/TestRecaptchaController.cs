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
    if (request == null || string.IsNullOrWhiteSpace(request.Token))
      return BadRequest("token_missing");

    var remoteIp = System.Web.HttpContext.Current?.Request?.UserHostAddress;
    var expectedHostname = System.Web.HttpContext.Current?.Request?.Url?.Host;
    var minimumScore = request.MinimumScore > 0 ? request.MinimumScore : -1;

    var result = await GetService<RecaptchaValidator>()
      .ValidateAsync(
        token: request.Token,
        remoteIp: remoteIp,
        minimumScore: minimumScore,
        expectedHostname: expectedHostname);

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