public class AccountController : Controller
{
    private readonly UserService _userService;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserService userService,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _userService = userService;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        SetAuthViewData();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        SetAuthViewData();

        if (!ModelState.IsValid) return View(vm);

        var user = await _userService.LoginAsync(vm.Email, vm.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return View(vm);
        }

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
            return View(vm);
        }

        await _userService.UpdateLastLoginAsync(user.Id);

        await RemoveRememberSessionCookieAsync();

        Session? rememberSession = null;
        if (vm.RememberMe)
        {
            rememberSession = await _userService.CreateRememberSessionAsync(user.Id);
            Response.Cookies.Append(
                UserService.RememberMeCookieName,
                rememberSession.SessionKey,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = rememberSession.ExpiresAt
                });
        }

        var principal = BuildUserPrincipal(user, rememberSession?.SessionKey);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = vm.RememberMe,
            ExpiresUtc = vm.RememberMe && rememberSession is not null
                ? new DateTimeOffset(DateTime.SpecifyKind(rememberSession.ExpiresAt, DateTimeKind.Utc))
                : null
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        HttpContext.Session.SetInt32("UserId", user.Id);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        SetAuthViewData();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        SetAuthViewData();

        if (!ModelState.IsValid) return View(vm);

        var email = vm.Email.Trim();
        var (success, error, user) = await _userService.RegisterAsync(vm.Name, email, vm.Password);
        if (!success)
        {
            var errorKey = error == UserService.MaxUserNameLengthError ? nameof(vm.Name) : nameof(vm.Email);
            ModelState.AddModelError(errorKey, error);
            return View(vm);
        }

        var token = await _userService.CreateEmailConfirmationTokenAsync(user!.Id);
        var confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Account",
            new { userId = user.Id, token },
            Request.Scheme);

        if (!string.IsNullOrWhiteSpace(confirmationLink))
        {
            var encodedLink = HtmlEncoder.Default.Encode(confirmationLink);
            try
            {
                await _emailSender.SendEmailAsync(
                    email,
                    "Confirm your Biolife account",
                    BuildConfirmationEmailHtml(vm.Name, encodedLink));
            }
            catch (SmtpException)
            {
                await _userService.DeleteUserByEmailAsync(email);

                ModelState.AddModelError(
                    string.Empty,
                    "Could not send confirmation email. Check Gmail SMTP settings and try again.");

                return View(vm);
            }
        }

        TempData["Success"] = "Registration successful. Please check your email and confirm your account.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(int userId, string token)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
        {
            return EmailConfirmationView(
                false,
                "Confirmation link is invalid",
                "This email confirmation link is missing required data. Please register again or request a new confirmation email.",
                "Back to Sign In",
                StatusCodes.Status400BadRequest);
        }

        var confirmed = await _userService.ConfirmEmailAsync(userId, token);
        if (confirmed)
        {
            return EmailConfirmationView(
                true,
                "Email confirmed",
                "Your account is ready. You can now sign in and continue to Biolife.",
                "Sign In");
        }

        return EmailConfirmationView(
            false,
            "Confirmation failed",
            "This confirmation link is expired or already used. Please register again to receive a fresh link.",
            "Back to Sign In",
            StatusCodes.Status400BadRequest);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        SetAuthViewData();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVm vm)
    {
        SetAuthViewData();

        if (!ModelState.IsValid)
            return View(vm);

        var email = vm.Email.Trim();
        var user = await _userService.FindPasswordResetUserByEmailAsync(email);
        if (user is not null)
        {
            var token = await _userService.CreatePasswordResetTokenAsync(user.Id);
            var resetLink = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { userId = user.Id, token },
                Request.Scheme);

            if (!string.IsNullOrWhiteSpace(resetLink))
            {
                var encodedLink = HtmlEncoder.Default.Encode(resetLink);

                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset your Biolife password",
                        BuildPasswordResetEmailHtml(user.Name, encodedLink));
                }
                catch (SmtpException)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Could not send password reset email. Check Gmail SMTP settings and try again.");

                    return View(vm);
                }
            }
        }

        TempData["Success"] = "If this email exists, we sent a password reset link. Please check your inbox.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(int userId, string token)
    {
        SetAuthViewData();

        if (userId <= 0 || string.IsNullOrWhiteSpace(token) ||
            !await _userService.IsPasswordResetTokenValidAsync(userId, token))
        {
            TempData["Error"] = "Password reset link is invalid or expired. Please request a new one.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordVm
        {
            UserId = userId,
            Token = token
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVm vm)
    {
        SetAuthViewData();

        if (!ModelState.IsValid)
            return View(vm);

        var reset = await _userService.ResetPasswordAsync(vm.UserId, vm.Token, vm.Password);
        if (!reset)
        {
            ModelState.AddModelError(string.Empty, "Password reset link is invalid or expired. Please request a new one.");
            return View(vm);
        }

        await RemoveRememberSessionCookieAsync();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Remove("UserId");

        TempData["Success"] = "Password changed successfully. You can sign in with your new password.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        if (!IsGoogleAuthConfigured())
        {
            return RedirectToAction(nameof(Login));
        }

        var safeReturnUrl = GetSafeReturnUrl(returnUrl);
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl = safeReturnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
    {
        var result = await HttpContext.AuthenticateAsync(UserService.ExternalCookieScheme);
        if (!result.Succeeded || result.Principal is null)
        {
            TempData["Error"] = "Could not complete Google login. Please try again.";
            return RedirectToAction(nameof(Login));
        }

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            await HttpContext.SignOutAsync(UserService.ExternalCookieScheme);
            TempData["Error"] = "Google did not return an email address.";
            return RedirectToAction(nameof(Login));
        }

        var name = result.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];
        var (success, error, user) = await _userService.FindOrCreateGoogleUserAsync(name, email);
        await HttpContext.SignOutAsync(UserService.ExternalCookieScheme);

        if (!success || user is null)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Login));
        }

        await _userService.UpdateLastLoginAsync(user.Id);
        await RemoveRememberSessionCookieAsync();

        var principal = BuildUserPrincipal(user);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false });

        HttpContext.Session.SetInt32("UserId", user.Id);

        return LocalRedirect(GetSafeReturnUrl(returnUrl));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await RemoveRememberSessionCookieAsync();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Remove("UserId");
        return RedirectToAction(nameof(Login));
    }

    private async Task RemoveRememberSessionCookieAsync()
    {
        var sessionKeysToRemove = new HashSet<string>(StringComparer.Ordinal);

        if (Request.Cookies.TryGetValue(UserService.RememberMeCookieName, out var sessionKey))
            sessionKeysToRemove.Add(sessionKey);

        var claimSessionKey = User.FindFirstValue(UserService.RememberSessionClaimType);
        if (!string.IsNullOrWhiteSpace(claimSessionKey))
            sessionKeysToRemove.Add(claimSessionKey);

        foreach (var key in sessionKeysToRemove)
            await _userService.RemoveSessionByKeyAsync(key);

        Response.Cookies.Delete(UserService.RememberMeCookieName);
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

    private IActionResult EmailConfirmationView(
        bool isSuccess,
        string title,
        string message,
        string actionText,
        int? statusCode = null)
    {
        if (statusCode.HasValue)
            Response.StatusCode = statusCode.Value;

        return View("ConfirmEmail", new EmailConfirmationVm
        {
            IsSuccess = isSuccess,
            Title = title,
            Message = message,
            ActionText = actionText
        });
    }

    private bool IsGoogleAuthConfigured()
    {
        var googleSection = _configuration.GetSection("Authentication:Google");
        return !string.IsNullOrWhiteSpace(googleSection["ClientId"])
            && !string.IsNullOrWhiteSpace(googleSection["ClientSecret"]);
    }

    private void SetAuthViewData()
    {
        ViewBag.IsGoogleAuthConfigured = IsGoogleAuthConfigured();
    }

    private string GetSafeReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action("Index", "Home") ?? "/";
    }

    private static string BuildConfirmationEmailHtml(string name, string encodedConfirmationLink)
    {
        var encodedName = HtmlEncoder.Default.Encode(name.Trim());

        return $"""
            <!doctype html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Confirm your Biolife account</title>
            </head>
            <body style="margin:0; padding:0; background:#f7fbf4; font-family:Montserrat, Arial, sans-serif; color:#24312f;">
                <div style="display:none; max-height:0; overflow:hidden; opacity:0;">Confirm your Biolife account and start using your profile.</div>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f7fbf4; padding:32px 14px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:520px; background:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #dbeed5; box-shadow:0 18px 44px rgba(5,165,3,0.12);">
                                <tr>
                                    <td style="padding:30px 32px 18px; border-top:5px solid #05a503; border-bottom:1px solid #dbeed5; background:#fbfff8;">
                                        <div style="font-size:23px; font-weight:700; color:#24312f; line-height:1.2;">Biolife<span style="color:#05a503;">Panel</span></div>
                                        <div style="margin-top:7px; font-size:13px; color:#70827f; font-weight:500;">Secure account confirmation</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:28px 32px 32px;">
                                        <div style="width:44px; height:44px; border-radius:8px; background:#e4f8df; color:#047d02; line-height:44px; text-align:center; font-size:22px; font-weight:700; margin-bottom:18px;">&#10003;</div>
                                        <h1 style="margin:0 0 10px; font-size:22px; line-height:1.3; color:#24312f;">Confirm your email</h1>
                                        <p style="margin:0 0 18px; font-size:14px; line-height:1.7; color:#70827f;">Hi {encodedName}, your account has been created. Please confirm this email address to activate your Biolife profile.</p>
                                        <a href="{encodedConfirmationLink}" style="display:block; width:100%; box-sizing:border-box; background:#05a503; color:#ffffff; text-decoration:none; text-align:center; padding:13px 20px; border-radius:8px; font-size:14px; font-weight:700;">Confirm Email</a>
                                        <p style="margin:20px 0 0; font-size:12px; line-height:1.6; color:#9aa89a;">This link expires in 24 hours. If you did not create this account, you can ignore this email.</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }

    private static string BuildPasswordResetEmailHtml(string name, string encodedResetLink)
    {
        var encodedName = HtmlEncoder.Default.Encode(name.Trim());

        return $"""
            <!doctype html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Reset your Biolife password</title>
            </head>
            <body style="margin:0; padding:0; background:#f7fbf4; font-family:Montserrat, Arial, sans-serif; color:#24312f;">
                <div style="display:none; max-height:0; overflow:hidden; opacity:0;">Use this secure link to reset your Biolife password.</div>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f7fbf4; padding:32px 14px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:520px; background:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #dbeed5; box-shadow:0 18px 44px rgba(5,165,3,0.12);">
                                <tr>
                                    <td style="padding:30px 32px 18px; border-top:5px solid #05a503; border-bottom:1px solid #dbeed5; background:#fbfff8;">
                                        <div style="font-size:23px; font-weight:700; color:#24312f; line-height:1.2;">Biolife<span style="color:#05a503;">Panel</span></div>
                                        <div style="margin-top:7px; font-size:13px; color:#70827f; font-weight:500;">Secure password reset</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:28px 32px 32px;">
                                        <div style="width:44px; height:44px; border-radius:8px; background:#e4f8df; color:#047d02; line-height:44px; text-align:center; font-size:22px; font-weight:700; margin-bottom:18px;">&#8635;</div>
                                        <h1 style="margin:0 0 10px; font-size:22px; line-height:1.3; color:#24312f;">Reset your password</h1>
                                        <p style="margin:0 0 18px; font-size:14px; line-height:1.7; color:#70827f;">Hi {encodedName}, we received a request to reset your Biolife password. Use the button below to choose a new password.</p>
                                        <a href="{encodedResetLink}" style="display:block; width:100%; box-sizing:border-box; background:#05a503; color:#ffffff; text-decoration:none; text-align:center; padding:13px 20px; border-radius:8px; font-size:14px; font-weight:700;">Reset Password</a>
                                        <p style="margin:20px 0 0; font-size:12px; line-height:1.6; color:#9aa89a;">This link expires in 1 hour. If you did not request a password reset, you can ignore this email.</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }
}
