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
  public async Task<IActionResult> Verify([FromBody] RecaptchaRequest request)
#else
  [HttpPost]
  [SecureEndpoint]
  public async Task<IHttpActionResult> Verify([FromBody] RecaptchaRequest request)
#endif
  {
    if (request == null || string.IsNullOrWhiteSpace(request.Token))
    {
#if NETCOREAPP
      return BadRequest(new { ok = false, error = "token_missing" });
#else
      return Json(new { ok = false, error = "token_missing" });
#endif
    }

    var privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value
                     ?? AllSettings.String("GoogleRecaptcha.PrivateKey");

    var minimumScoreFromSettings = AllSettings.Double("GoogleRecaptcha.ScoreThreshold");

    var minimumScore = request.MinimumScore > 0
      ? request.MinimumScore
      : minimumScoreFromSettings;


    var remoteIp = System.Web.HttpContext.Current?.Request?.UserHostAddress;

    var validator = new RecaptchaValidator();
    var result = await validator.ValidateAsync(
      token: request.Token,
      privateKey: privateKey,
      remoteIp: remoteIp,
      minimumScore: minimumScore,
      expectedHostname: null
    );

    var response = new
    {
      ok = result.IsValid,
      isValid = result.IsValid,
      score = result.Score,
      hostname = result.Hostname,
      error = result.Error,
      errorCodes = result.ErrorCodes
    };

#if NETCOREAPP
    return Ok(response);
#else
    return Json(response);
#endif
  }
}

public class RecaptchaRequest
{
  public string Token { get; set; }
  public double MinimumScore { get; set; } = 0;
}