#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

using ToSic.Sxc.WebApi;

[AllowAnonymous]
public class DemoFormControllerHybrid : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  [SecureEndpoint]
  public IActionResult Submit([FromBody] DemoFormRequest request)
  {
    if (request == null || string.IsNullOrWhiteSpace(request.Message))
    {
      return BadRequest("message_missing");
    }
#if NETCOREAPP
    return Ok(new { ok = true });
#else
    return Json(new { ok = true });
#endif
  }
}