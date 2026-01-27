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
    {
      var errResponse = new { ok = false, error = "token_missing" }; 
      
      return Json(errResponse);
    }
    var remoteIp = System.Web.HttpContext.Current?.Request?.UserHostAddress;

    var validator = GetService<RecaptchaValidator>();
    var result = await validator.ValidateAsync(token: request.Token, remoteIp: remoteIp, expectedHostname: null);

    var response = new
    {
      Success = result.Success,
      score = result.Score,
      demoHostname = result.Hostname,
      error = result.Error,
      errorCodes = result.ErrorCodes
    };
    return Json(response);
  }
}

public class RecaptchaRequest
{
  public string Token { get; set; }
  public double MinimumScore { get; set; } = 0;
}