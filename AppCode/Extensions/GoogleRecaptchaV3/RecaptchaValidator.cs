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
    // Google verify endpoint (Uri is created only when used)
    private const string SiteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    /// <summary>
    /// Validate a reCAPTCHA v3 token.
    /// </summary>
    /// <param name="token">Client-side reCAPTCHA token</param>
    /// <param name="privateKey">Optional private key (loaded from settings if null)</param>
    /// <param name="remoteIp">Optional user IP</param>
    /// <param name="minimumScore">Minimum accepted score (0–1)</param>
    /// <param name="expectedHostname">Optional hostname check</param>
    public async Task<RecaptchaResult> ValidateAsync(
      string token,
      string privateKey = null,
      string remoteIp = null,
      double minimumScore = -1,
      string expectedHostname = null
    )
    {
      #region Input validation

      if (string.IsNullOrWhiteSpace(token))
        return RecaptchaResult.Err("token_missing");

      if (string.IsNullOrWhiteSpace(privateKey))
      {
        privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value;

        if (string.IsNullOrWhiteSpace(privateKey))
          return RecaptchaResult.Err("private_key_missing");
      }

      if (minimumScore < 0 || minimumScore > 1)
      {
        minimumScore = AllSettings.Double("GoogleRecaptcha.ScoreThreshold");
        if (minimumScore < 0 || minimumScore > 1)
          return RecaptchaResult.Err("invalid_minimum_score");
      }

      #endregion

      // Send verification request to Google
      using var httpClient = new HttpClient();

      var form = new Dictionary<string, string>
      {
        { "secret", privateKey },
        { "response", token }
      };

      if (!string.IsNullOrWhiteSpace(remoteIp))
        form.Add("remoteip", remoteIp);

      var response = await httpClient
        .PostAsync(new Uri(SiteVerifyUrl), new FormUrlEncodedContent(form))
        .ConfigureAwait(false);

      var body = await response.Content.ReadAsStringAsync()
        .ConfigureAwait(false);

      // Parse Google response
      RecaptchaResponse captchaResponse;
      try
      {
        captchaResponse = Kit.Json.To<RecaptchaResponse>(body);
      }
      catch (Exception ex)
      {
        return RecaptchaResult.Err(ex.Message);
      }

      if (captchaResponse == null)
        return RecaptchaResult.Err("empty_response");

      // Google rejected token
      if (!captchaResponse.Success)
      {
        Kit.Page.SetHttpStatus(400, "captcha_failed");
        return RecaptchaResult.Err("captcha_failed");
      }

      // Optional hostname validation
      if (!string.IsNullOrWhiteSpace(expectedHostname) &&
          !string.Equals(
            captchaResponse.Hostname,
            expectedHostname,
            StringComparison.OrdinalIgnoreCase
          ))
        return RecaptchaResult.Err("hostname_mismatch");

      // Optional score validation
      if (captchaResponse.Score.HasValue &&
          minimumScore > 0 &&
          captchaResponse.Score.Value < minimumScore)
        return RecaptchaResult.Err("score_too_low");

      return new RecaptchaResult
      {
        IsValid = true,
        Score = captchaResponse.Score,
        Hostname = captchaResponse.Hostname,
        ErrorCodes = captchaResponse.ErrorCodes
      };
    }
  }
}
