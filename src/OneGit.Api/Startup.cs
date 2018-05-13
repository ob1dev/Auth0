using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneGit.Api.Authorization;
using OneGit.Api.Data;

namespace OneGit.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<RepositoryContext>(options =>
        options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      // 1. Add Authentication Services
      string domain = $"https://{this.Configuration["Auth0:Domain"]}/";
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.Authority = domain;
        options.Audience = this.Configuration["Auth0:ApiIdentifier"];
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
      else
      {
        app.UseHsts();
      }

      app.UseAuthentication();

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}