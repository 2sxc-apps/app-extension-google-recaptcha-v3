namespace AppCode.Extensions.GoogleRecaptchaV3
{
  /// <summary>
  /// Response vom Google reCAPTCHA /siteverify Endpoint
  /// </summary>
  public class RecaptchaResponse // TODO: @2rb - TRY TO MAKE INTERNAL
  {
    public bool Success { get; set; }
    public double? Score { get; set; }
    public string Challenge_ts { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
  }
}
