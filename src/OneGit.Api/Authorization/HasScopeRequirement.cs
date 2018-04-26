using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneGit.Api.Authorization
{
  public class HasScopeRequirement : IAuthorizationRequirement
  {
    public string Issuer { get; }
    public string Scope { get; }

    public HasScopeRequirement(string scope, string issuer)
    {
      Scope = scope ?? throw new ArgumentNullException(nameof(scope), $"The parameter '{nameof(scope)}' cannot be null.");
      Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer), $"The parameter '{nameof(issuer)}' cannot be null.");
    }
  }
}