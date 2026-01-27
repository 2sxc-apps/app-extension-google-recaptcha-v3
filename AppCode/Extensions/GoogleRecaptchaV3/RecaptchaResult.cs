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
      => new RecaptchaResult { Success = false, Error = errorCode };

    public RecaptchaResult ToError(string errorCode)
    {
      Success = false;
      Error = errorCode;
      return this;
    }

    public bool Success { get; set; }
    public double? Score { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
    public string Error { get; set; }
  }

  public static class RecaptchaErrors
{
  public const string MissingToken = "missing-token";
  public const string InvalidResponse = "invalid-response";
  public const string ScoreTooLow = "score-too-low";
  public const string ActionMismatch = "action-mismatch";
}

}
