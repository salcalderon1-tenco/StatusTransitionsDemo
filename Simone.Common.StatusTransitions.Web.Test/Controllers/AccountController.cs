using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simone.Common.StatusTransitions.Web.Test.Controllers
{
    /// <summary>
    /// Handles the user's session for the demo app. Sign-in is driven by the OpenIdConnect
    /// challenge configured by <c>AddSimoneAuthorization</c>, so this controller only needs to
    /// expose the sign-out endpoint used by the navbar's logout button.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Signs the current user out. 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
