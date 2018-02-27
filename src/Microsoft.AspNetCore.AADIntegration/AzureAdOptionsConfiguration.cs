// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System.Linq;
using Microsoft.AspNetCore.AADIntegration;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication
{
    internal class AzureAdOptionsConfiguration : IConfigureNamedOptions<AzureAdOptions>
    {
        private readonly IOptions<AzureAdSchemeOptions> _schemeOptions;

        public AzureAdOptionsConfiguration(IOptions<AzureAdSchemeOptions> schemeOptions)
        {
            _schemeOptions = schemeOptions;
        }

        public void Configure(string name, AzureAdOptions options)
        {
            for (var i = 0; i < _schemeOptions.Value.Mappings.Count; i++)
            {
                var mapping = _schemeOptions.Value.Mappings[i];
                if (mapping.AADScheme == name)
                {
                    options.OpenIdConnectSchemeName = mapping.OpenIdConnectScheme;
                    options.CookieSchemeName = mapping.CookieScheme;
                }
            }
        }

        public void Configure(AzureAdOptions options)
        {
        }
    }
}