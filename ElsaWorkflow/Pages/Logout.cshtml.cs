using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Flurl;

namespace ElsaWorkflow.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration configuration;

        public LogoutModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            //await HttpContext.SignOutAsync();

            var redirect_uri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var url = new Uri(configuration["keycloak:auth-server-url"])
                .AppendPathSegments("realms", configuration["keycloak:realm"], "/protocol/openid-connect/logout")
                .SetQueryParam("post_logout_redirect_uri", redirect_uri)
                .SetQueryParam("client_id", configuration["keycloak:resource"] + "-cli");
            return Redirect(url.ToString());
        }
    }
}
