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
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

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

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info
        {
          Version = "v1",
          Title = "Repositories API",
          Description = "A simple example ASP.NET Core Web API",
          TermsOfService = "None",
          Contact = new Contact
          {
            Name = "Oleg Burov",
            Email = "oleg.burov@outlook.com",
            Url = "https://twitter.com/oleg_burov"
          },
          License = new License
          {
            Name = "MIT",
            Url = "https://github.com/olegburov/Auth0/blob/master/LICENSE"
          }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
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

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Repositories API V1");
        c.RoutePrefix = string.Empty;
      });

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}