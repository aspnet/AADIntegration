// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.AADIntegration
{
    internal class OpenIdConnectOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IOptions<AzureAdSchemeOptions> _schemeOptions;
        private readonly IOptionsMonitor<AzureAdOptions> _Options;

        public OpenIdConnectOptionsConfiguration(IOptions<AzureAdSchemeOptions> schemeOptions, IOptionsMonitor<AzureAdOptions> Options)
        {
            _schemeOptions = schemeOptions;
            _Options = Options;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            var _Scheme = GetScheme(name);
            var Options = _Options.Get(_Scheme);
            if (name != Options.OpenIdConnectSchemeName)
            {
                return;
            }

            options.ClientId = Options.ClientId;
            options.Authority = $"{Options.Instance}{Options.TenantId}";
            options.UseTokenLifetime = true;
            options.CallbackPath = Options.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = Options.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = Options.CookieSchemeName;
        }

        private string GetScheme(string name)
        {
            for (var i = 0; i < _schemeOptions.Value.Mappings.Count; i++)
            {
                var mapping = _schemeOptions.Value.Mappings[i];
                if (mapping.OpenIdConnectScheme == name)
                {
                    return mapping.AADScheme;
                }
            }

            return null;
        }

        public void Configure(OpenIdConnectOptions options)
        {
        }
    }
}
