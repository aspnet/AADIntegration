// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.B2CIntegration
{
    internal class B2COptionsConfiguration : IConfigureNamedOptions<AzureAdB2COptions>
    {
        private readonly IOptions<AzureAdB2CSchemeOptions> _schemeOptions;

        public B2COptionsConfiguration(IOptions<AzureAdB2CSchemeOptions> schemeOptions)
        {
            _schemeOptions = schemeOptions;
        }

        public void Configure(string name, AzureAdB2COptions options)
        {
            for (var i = 0; i < _schemeOptions.Value.Mappings.Count; i++)
            {
                var mapping = _schemeOptions.Value.Mappings[i];
                if (mapping.B2CScheme == name)
                {
                    options.OpenIdConnectSchemeName = mapping.OpenIdConnectScheme;
                    options.CookieSchemeName = mapping.CookieScheme;
                }
            }
        }

        public void Configure(AzureAdB2COptions options)
        {
        }
    }
}
