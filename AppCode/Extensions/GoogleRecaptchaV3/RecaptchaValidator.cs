using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppCode.Extensions.GoogleRecaptchaV3
{
  /// <summary>
  /// Google reCAPTCHA v3 validator.
  /// Verifies token, score, and optional hostname.
  /// </summary>
  public class RecaptchaValidator : Custom.Hybrid.CodeTyped
  {
    private const string SiteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    public async Task<RecaptchaResult> ValidateAsync(string token, string privateKey = null,
    string remoteIp = null,
    double minimumScore = -1,
    string expectedHostname = null)
    {
      if (string.IsNullOrWhiteSpace(token))
        return RecaptchaResult.Err(RecaptchaErrors.MissingToken);

      var secret = !string.IsNullOrWhiteSpace(privateKey)? privateKey : AllSettings.String("GoogleRecaptcha.PrivateKey", required: false);

      if (string.IsNullOrWhiteSpace(secret))
      {
        return RecaptchaResult.Err("missing-secret");
    }
      // Minimum score fallback 
      if (minimumScore < 0 || minimumScore > 1)
      {
        minimumScore = AllSettings.Double("GoogleRecaptcha.ScoreThreshold", fallback: 0.5);
      }
      using var httpClient = new HttpClient();

      var form = new Dictionary<string, string>
      {
        { "secret", secret },
        { "response", token }
      };

      if (!string.IsNullOrWhiteSpace(remoteIp))
      {
        form.Add("remoteip", remoteIp);
      }
      var httpResponse = await httpClient.PostAsync(
        new Uri(SiteVerifyUrl),
        new FormUrlEncodedContent(form)
      ).ConfigureAwait(false);

      var body = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

      if (string.IsNullOrWhiteSpace(body))
      {
        return RecaptchaResult.Err("empty-response");
      
      }
      RecaptchaResult google;
      try
      {
        google = Kit.Json.To<RecaptchaResult>(body);
      }
      catch
      {
        return RecaptchaResult.Err(RecaptchaErrors.InvalidResponse);
      }

      // If Google rejected, return their error codes
      if (!google.Success)
      {
        Kit.Page.SetHttpStatus(400, "captcha-failed");
        return google.ToError(string.Join(",", google.ErrorCodes));
      }

      if (!string.IsNullOrWhiteSpace(expectedHostname) && !string.Equals(google.Hostname, expectedHostname, StringComparison.OrdinalIgnoreCase))
      {
        return google.ToError(RecaptchaErrors.ActionMismatch);
      }
      if (google.Score.HasValue && minimumScore > 0 && google.Score.Value < minimumScore)
      {
        return google.ToError(RecaptchaErrors.ScoreTooLow);
      }
      return google;
    }
  }
}
