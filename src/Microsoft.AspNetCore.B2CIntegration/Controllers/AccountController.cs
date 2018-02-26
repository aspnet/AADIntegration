// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.B2CIntegration.Controllers
{
    [NonController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IOptionsMonitor<AzureAdB2COptions> _options;

        public AccountController(IOptionsMonitor<AzureAdB2COptions> b2cOptions)
        {
            _options = b2cOptions;
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignIn(string scheme)
        {
            scheme = scheme ?? AzureAdB2CDefaults.AuthenticationScheme;
            var options = _options.Get(scheme);

            var redirectUrl = Url.Content("~/");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                scheme);
        }

        [HttpGet]
        public IActionResult ResetPassword(string scheme)
        {
            scheme = scheme ?? AzureAdB2CDefaults.AuthenticationScheme;
            var options = _options.Get(scheme);

            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = options.ResetPasswordPolicyId;
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult EditProfile(string scheme)
        {
            scheme = scheme ?? AzureAdB2CDefaults.AuthenticationScheme;
            var options = _options.Get(scheme);

            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = options.EditProfilePolicyId;
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignOut(string scheme)
        {
            var options = _options.Get(scheme ?? AzureAdB2CDefaults.AuthenticationScheme);

            var callbackUrl = Url.Page("/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                options.CookieSchemeName,
                options.OpenIdConnectSchemeName);
        }
    }
}
