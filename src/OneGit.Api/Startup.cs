using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneGit.Api.Authorization;

namespace OneGit.Api
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
      services.AddMvc();

      // 1. Add Authentication Services
      string domain = $"https://{Configuration["Auth0:Domain"]}/";
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      }).AddJwtBearer(options =>
      {
        options.Authority = domain;
        options.Audience = Configuration["Auth0:ApiIdentifier"];
      });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("read:repositories", policy => policy.Requirements.Add(new HasScopeRequirement("read:repositories", domain)));
        options.AddPolicy("create:repositories", policy => policy.Requirements.Add(new HasScopeRequirement("create:repositories", domain)));
        options.AddPolicy("update:repositories", policy => policy.Requirements.Add(new HasScopeRequirement("update:repositories", domain)));
        options.AddPolicy("delete:repositories", policy => policy.Requirements.Add(new HasScopeRequirement("delete:repositories", domain)));
      });

      // register the scope authorization handler
      services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // 2. Enable authentication middleware
      app.UseAuthentication();

      app.UseMvc();
    }
  }
}