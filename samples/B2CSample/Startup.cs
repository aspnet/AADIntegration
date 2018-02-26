// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.B2CIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2CSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o => o.DefaultScheme = "B2C")
                .AddAzureAdB2C(options => Configuration.GetSection("AzureAdB2C").Bind(options))
                .AddAzureAdB2C("B2C", "OpenId", "Cookie", "B2C", options =>
                  {
                      options.Instance = "https://login.microsoftonline.com/tfp/";
                      options.ClientId = "64f31f76-2750-49e4-aab9-f5de105b5172";
                      options.CallbackPath = "/signin-oidc-b2c";
                      options.SignedOutCallbackPath = "/signout-callback-oidc-b2c";
                      options.Domain = "jacalvarb2c.onmicrosoft.com";
                      options.SignUpSignInPolicyId = "B2C_1_SiUpIn";
                      options.ResetPasswordPolicyId = "B2C_1_SSPR";
                      options.EditProfilePolicyId = "B2C_1_SiPe";
                  });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}