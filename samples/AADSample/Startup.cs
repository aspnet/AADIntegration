// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.AADIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AADSample
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
            services.AddAuthentication(AzureAdDefaults.AuthenticationScheme)
                .AddAzureAd(options => Configuration.GetSection("AzureAd").Bind(options))
                .AddAzureAd("AD", "OpenId", "Cookie", "Azure Active Directory", options =>
                  {
                      options.Instance = "https://login.microsoftonline.com/";
                      options.Domain = "jacalvaraadtest1.onmicrosoft.com";
                      options.TenantId = "7e511586-66ec-4108-bc9c-a68dee0dc2aa";
                      options.ClientId = "db33c356-12cc-4953-9167-00ad56c2e8b2";
                      options.CallbackPath = "/signin-oidc-ad";
                      options.SignedOutCallbackPath = "/signout-callback-oidc-ad";
                  })
                 .AddAzureAd(
                    "Multi",
                    "MultiOpenID",
                    "MultiCookie",
                    "Multi AAD",
                    o => Configuration.GetSection("AzureAdMultiOrg").Bind(o));

            services.Configure<OpenIdConnectOptions>("MultiOpenID", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                         // If your authentication logic is based on users then add your logic here
                         return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                         return Task.CompletedTask;
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
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