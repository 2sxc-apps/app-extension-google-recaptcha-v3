using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AppCode.Extensions.GoogleRecaptchaV3.RecaptchaValidator;

namespace AppCode.Extensions.GoogleRecaptchaV3.Recaptcha
{
  public class RecaptchaValidator
  {
    private static readonly Uri SiteVerifyUri =
      new Uri("https://www.google.com/recaptcha/api/siteverify");

    private static readonly JsonSerializerOptions JsonOptions =
      new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public async Task<RecaptchaResult> ValidateAsync(
      string token,
      string privateKey,
      string remoteIp = null,
      double minimumScore = 0.5,
      string expectedHostname = null
    )
    {
      if (string.IsNullOrWhiteSpace(token))
        return new RecaptchaResult { IsValid = false, Error = "token_missing" };

      if (string.IsNullOrWhiteSpace(privateKey))
        return new RecaptchaResult { IsValid = false, Error = "private_key_missing" };

      using (var httpClient = new HttpClient())
      {
        var form = new List<KeyValuePair<string, string>>
        {
          new KeyValuePair<string, string>("secret", privateKey),
          new KeyValuePair<string, string>("response", token)
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
          form.Add(new KeyValuePair<string, string>("remoteip", remoteIp));

        var response = await httpClient
          .PostAsync(SiteVerifyUri, new FormUrlEncodedContent(form))
          .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        RecaptchaResponse captchaResponse;
        try
        {
          captchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(body, JsonOptions);
        }
        catch (Exception ex)
        {
          return new RecaptchaResult
          {
            IsValid = false,
            Error = "json_parse_failed",
            ErrorCodes = new[] { ex.Message }
          };
        }

        if (captchaResponse == null)
          return new RecaptchaResult { IsValid = false, Error = "empty_response" };

        var result = new RecaptchaResult
        {
          IsValid = captchaResponse.Success,
          Score = captchaResponse.Score,
          Hostname = captchaResponse.Hostname,
          ErrorCodes = captchaResponse.ErrorCodes
        };

        if (!captchaResponse.Success)
          return result;

        if (!string.IsNullOrWhiteSpace(expectedHostname) &&
            !string.Equals(captchaResponse.Hostname, expectedHostname, StringComparison.OrdinalIgnoreCase))
        {
          result.IsValid = false;
          result.Error = "hostname_mismatch";
          return result;
        }

        if (captchaResponse.Score.HasValue &&
            minimumScore > 0 &&
            captchaResponse.Score.Value < minimumScore)
        {
          result.IsValid = false;
          result.Error = "score_too_low";
          return result;
        }

        return result;
      }
    }
  }
}
