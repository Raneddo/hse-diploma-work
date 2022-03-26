using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("")]
public class SwaggerRedirect
{
    [HttpGet]
    public IActionResult RedirectSwagger()
    {
        return new RedirectResult("/swagger/index.html");
    }
}