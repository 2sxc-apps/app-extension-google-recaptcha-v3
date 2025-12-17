using System.Text.Json.Serialization;

namespace AppCode.Extensions.GoogleRecaptchaV3.RecaptchaValidator
{
  // Response vom Google reCAPTCHA siteverify Endpoint
  public class RecaptchaResponse
  {
    public bool Success { get; set; }
    public double? Score { get; set; }
    public string Challenge_ts { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
  }
}
