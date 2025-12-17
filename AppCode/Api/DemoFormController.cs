#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

[AllowAnonymous]
public class DemoFormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  public IActionResult Submit([FromBody] DemoFormRequest request)
  {
    // First validate the request with Recaptcha
    

    if (request == null || string.IsNullOrWhiteSpace(request.Message))
      return Json(new { ok = false, error = "message_missing" });
    
    return Json(new { ok = true });
  }
}

public class DemoFormRequest
{
  public string Message { get; set; }
}
