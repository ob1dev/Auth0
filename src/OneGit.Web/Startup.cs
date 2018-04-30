﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OneGit.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneGit.Web
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
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      // Add authentication services
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie(options => options.LoginPath = "/Account/Signin")
      .AddOpenIdConnect("Auth0", options =>
      {
        // Set the authority to your Auth0 domain
        options.Authority = $"https://{Configuration["Auth0:Domain"]}";

        // Configure the Auth0 Client ID and Client Secret
        options.ClientId = Configuration["Auth0:ClientId"];
        options.ClientSecret = Configuration["Auth0:ClientSecret"];

        // Set response type to code
        options.ResponseType = "code";

        // Configure the scope
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        var apiScopes = string.Join(" ", Configuration.GetSection("Auth0:ApiScopes").GetChildren().Select(s => s.Value));
        options.Scope.Add(apiScopes);

        // Set the correct name claim type
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "nickname",
          RoleClaimType = "https://schemas.quickstarts.com/roles"
        };

        // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
        options.CallbackPath = new PathString("/signin-auth0");

        // Configure the Claims Issuer to be Auth0
        options.ClaimsIssuer = "Auth0";

        // Saves tokens to the AuthenticationProperties
        options.SaveTokens = true;

        options.Events = new OpenIdConnectEvents
        {
          // handle the logout redirection 
          OnRedirectToIdentityProviderForSignOut = (context) =>
            {
              var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

              var postLogoutUri = context.Properties.RedirectUri;
              if (!string.IsNullOrEmpty(postLogoutUri))
              {
                if (postLogoutUri.StartsWith("/"))
                {
                  // transform to absolute
                  var request = context.Request;
                  postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }

                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
              }

              context.Response.Redirect(logoutUri);
              context.HandleResponse();

              return Task.CompletedTask;
            },

          OnRedirectToIdentityProvider = context =>
          {
            context.ProtocolMessage.SetParameter("audience", Configuration["Auth0:ApiIdentifier"]);

            return Task.FromResult(0);
          }
        };
      });

      // Add framework services.
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddHttpClient<RepositoryClient>(client => client.BaseAddress = new Uri(Configuration["Auth0:ApiBaseUrl"]));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}