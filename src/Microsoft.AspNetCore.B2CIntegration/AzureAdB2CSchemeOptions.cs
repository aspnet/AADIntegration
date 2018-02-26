using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.B2CIntegration
{
    class AzureAdB2CSchemeOptions
    {
        public IList<B2CSchemeMapping> Mappings { get; set; } = new List<B2CSchemeMapping>();

        public class B2CSchemeMapping
        {
            public string B2CScheme { get; set; }
            public string OpenIdConnectScheme { get; set; }
            public string CookieScheme { get; set; }
        }
    }
}
