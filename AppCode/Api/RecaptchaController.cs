#if NETCOREAPP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

using System.Threading.Tasks;
using ToSic.Sxc.WebApi;
using AppCode.Extensions.GoogleRecaptchaV3.Recaptcha;

[AllowAnonymous]
public class RecaptchaController : Custom.Hybrid.ApiTyped
{
#if NETCOREAPP
  [HttpPost]
  [SecureEndpoint]
  public async Task<IActionResult> Verify([FromBody] RecaptchaVerifyRequest request)
#else
  [HttpPost]
  [SecureEndpoint]
  public async Task Verify([FromBody] RecaptchaVerifyRequest request)
#endif
  {
    if (request == null || string.IsNullOrWhiteSpace(request.Token))
    {
#if NETCOREAPP
      return BadRequest(new { ok = false, error = "token_missing" });
#else
      throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
#endif
    }
    var remoteIp =
#if NETCOREAPP
      Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
#else
      System.Web.HttpContext.Current?.Request?.UserHostAddress;
#endif

    var privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value;
    var minimumScore = AllSettings.Double("GoogleRecaptcha.ScoreThreshold");

    string expectedHostname = null;

    var validator = new RecaptchaValidator();

    var result = await validator.ValidateAsync(
      token: request.Token,
      privateKey: privateKey,
      remoteIp: remoteIp,
      minimumScore: minimumScore,
      expectedHostname: expectedHostname
    );

#if NETCOREAPP
    return Ok(new { ok = result.IsValid, score = result.Score, error = result.Error, hostname = result.Hostname });
#endif
  }
}

public class RecaptchaVerifyRequest
{
  public string Token { get; set; }
}
