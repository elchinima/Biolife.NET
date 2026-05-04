namespace Biolife.Infrastructure.Services
{
    public class SessionAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            await userService.DeleteExpiredSessionsAsync();

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var rememberSessionKey = context.User.FindFirstValue(UserService.RememberSessionClaimType);
                if (!string.IsNullOrWhiteSpace(rememberSessionKey))
                {
                    var rememberedUser = await userService.LoginBySessionKeyAsync(rememberSessionKey);
                    var currentUserId = GetCurrentUserId(context);

                    if (rememberedUser is null || currentUserId != rememberedUser.Id)
                    {
                        await InvalidateRememberLoginAsync(context, userService, rememberSessionKey);
                    }
                }

                if (context.User.Identity?.IsAuthenticated == true)
                {
                    EnsureSessionUserId(context);
                    await _next(context);
                    return;
                }
            }

            if (context.Request.Cookies.TryGetValue(UserService.RememberMeCookieName, out var sessionKey)
                && !string.IsNullOrWhiteSpace(sessionKey))
            {
                var user = await userService.LoginBySessionKeyAsync(sessionKey);

                if (user is null)
                {
                    context.Response.Cookies.Delete(UserService.RememberMeCookieName);
                }
                else
                {
                    await userService.UpdateLastLoginAsync(user.Id);

                    var principal = BuildUserPrincipal(user, sessionKey);
                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    context.Session.SetInt32("UserId", user.Id);
                }
            }

            await _next(context);
        }

        private static int? GetCurrentUserId(HttpContext context)
        {
            var claimValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out var userId) ? userId : null;
        }

        private static async Task InvalidateRememberLoginAsync(HttpContext context, UserService userService, string rememberSessionKey)
        {
            await userService.RemoveSessionByKeyAsync(rememberSessionKey);
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Session.Remove("UserId");
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
            context.Response.Cookies.Delete(UserService.RememberMeCookieName);
        }

        private static ClaimsPrincipal BuildUserPrincipal(User user, string? rememberSessionKey = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (!string.IsNullOrWhiteSpace(rememberSessionKey))
                claims.Add(new Claim(UserService.RememberSessionClaimType, rememberSessionKey));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        private static void EnsureSessionUserId(HttpContext context)
        {
            if (context.Session.GetInt32("UserId").HasValue)
                return;

            var claimValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimValue, out var userId))
                context.Session.SetInt32("UserId", userId);
        }
    }
}
