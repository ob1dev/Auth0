using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneGit.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OneGit.Web.Controllers
{
  [Route("[controller]/[action]")]
  public class AccountController : Controller
  {
    private TelemetryClient telemetry;

    public AccountController(TelemetryClient telemetry)
    {
      this.telemetry = telemetry;
    }

    [HttpGet]
    public async Task Signin(string returnUrl = "/")
    {
      await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties()
        {
          RedirectUri = returnUrl
        });

      this.telemetry.TrackEvent(new EventTelemetry("Signing in User"));
    }

    [HttpGet]
    [Authorize]
    public async Task Signout()
    {
      await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
        {
          // Indicate here where Auth0 should redirect the user after a logout.
          // Note that the resulting absolute Uri must be whitelisted in the 
          // **Allowed Logout URLs** settings for the app.
          RedirectUri = Url.Action("Index", "Home")
        });

      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      this.telemetry.TrackEvent(new EventTelemetry("Signing out User"));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
      return View(new UserProfileModel()
      {
        Name = User.Identity.Name,
        EmailAddress = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
        ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
        IdToken = await HttpContext.GetTokenAsync("id_token"),
        AccessToken = await HttpContext.GetTokenAsync("access_token")
      });
    }

    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}