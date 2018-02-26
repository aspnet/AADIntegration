// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.B2CIntegration
{
    internal class OpenIdConnectOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IOptions<AzureAdB2CSchemeOptions> _schemeOptions;
        private readonly IOptionsMonitor<AzureAdB2COptions> _b2COptions;

        public OpenIdConnectOptionsConfiguration(IOptions<AzureAdB2CSchemeOptions> schemeOptions, IOptionsMonitor<AzureAdB2COptions> b2cOptions)
        {
            _schemeOptions = schemeOptions;
            _b2COptions = b2cOptions;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            var _b2cScheme = GetB2cScheme(name);
            var b2COptions = _b2COptions.Get(_b2cScheme);
            if (name != b2COptions.OpenIdConnectSchemeName)
            {
                return;
            }

            options.ClientId = b2COptions.ClientId;
            options.Authority = $"{b2COptions.Instance}/{b2COptions.Domain}/{b2COptions.SignUpSignInPolicyId}/v2.0";
            options.UseTokenLifetime = true;
            options.CallbackPath = b2COptions.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = b2COptions.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = b2COptions.CookieSchemeName;

            options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = OnRedirectToIdentityProvider(b2COptions),
                OnRemoteFailure = OnRemoteFailure
            };
        }

        private string GetB2cScheme(string name)
        {
            for (var i = 0; i < _schemeOptions.Value.Mappings.Count; i++)
            {
                var mapping = _schemeOptions.Value.Mappings[i];
                if (mapping.OpenIdConnectScheme == name)
                {
                    return mapping.B2CScheme;
                }
            }

            return null;
        }

        public void Configure(OpenIdConnectOptions options)
        {
        }

        public Func<RedirectContext, Task> OnRedirectToIdentityProvider(AzureAdB2COptions b2cOptions)
        {
            return OnRedirectToIdentityProvider;

            Task OnRedirectToIdentityProvider(RedirectContext context)
            {

                var defaultPolicy = b2cOptions.DefaultPolicy;
                if (context.Properties.Items.TryGetValue(AzureAdB2COptions.PolicyAuthenticationProperty, out var policy) &&
                    !policy.Equals(defaultPolicy))
                {
                    context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                    context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower()
                        .Replace($"/{defaultPolicy.ToLower()}/", $"/{policy.ToLower()}/");
                    context.Properties.Items.Remove(AzureAdB2COptions.PolicyAuthenticationProperty);
                }

                return Task.CompletedTask;
            }
        }

        public Task OnRemoteFailure(RemoteFailureContext context)
        {
            context.HandleResponse();
            // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
            // because password reset is not supported by a "sign-up or sign-in policy"
            if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                context.Response.Redirect("/Account/ResetPassword");
            }
            else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
            {
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Redirect("/Error");
            }
            return Task.CompletedTask;
        }
    }
}
