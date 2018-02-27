using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.AADIntegration
{
    class AzureAdSchemeOptions
    {
        public IList<AADSchemeMapping> Mappings { get; set; } = new List<AADSchemeMapping>();

        public class AADSchemeMapping
        {
            public string AADScheme { get; set; }
            public string OpenIdConnectScheme { get; set; }
            public string CookieScheme { get; set; }
        }
    }
}
