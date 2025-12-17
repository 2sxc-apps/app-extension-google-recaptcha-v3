namespace AppCode.Extensions.GoogleRecaptchaV3.RecaptchaValidator
{
  // Response vom Google reCAPTCHA siteverify Endpoint
  public class RecaptchaResponse
  {
    public bool Success { get; set; }
    public double? Score { get; set; }          // v3 
    public string Challenge_ts { get; set; }    // timestamp
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
  }
}
