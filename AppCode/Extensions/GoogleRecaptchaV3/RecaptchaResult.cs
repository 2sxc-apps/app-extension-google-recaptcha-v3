namespace AppCode.Extensions.GoogleRecaptchaV3
{
  public class RecaptchaResult
  {
    /// <summary>
    /// Shorthand to generate an error-result
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    public static RecaptchaResult Err(string errorCode) 
      => new RecaptchaResult { IsValid = false, Error = errorCode };

    public RecaptchaResult ToError(string errorCode)
    {
      IsValid = false;
      Error = errorCode;
      return this;
    }

    public bool IsValid { get; set; }
    public double? Score { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
    public string Error { get; set; }
  }
}
