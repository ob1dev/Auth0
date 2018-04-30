using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OneGit.Web.Services
{
  public class RepositoryClient
  {
    private HttpClient client;
    private ILogger<RepositoryClient> logger;
    private IHttpContextAccessor httpContextAccessor;

    public RepositoryClient(HttpClient client, ILogger<RepositoryClient> logger, IHttpContextAccessor httpContextAccessor)
    {
      this.client = client;
      this.logger = logger;
      this.httpContextAccessor = httpContextAccessor;

      var context = this.httpContextAccessor.HttpContext;
      var token = context.GetTokenAsync("access_token").Result;

      if (token != null)
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      }
    }

    public async Task<IEnumerable<RepositoryModel>> GetAllRepositoriesAsync()
    {
      try
      {
        var response = await client.GetAsync("api/repositories");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<IEnumerable<RepositoryModel>>();
      }
      catch (HttpRequestException ex)
      {
        logger.LogError($"An error occured connecting to values API {ex.ToString()}");
        return Enumerable.Empty<RepositoryModel>();
      }
    }

    public async Task<RepositoryModel> GetRepositoryAsync(Guid id)
    {
      try
      {
        var response = await client.GetAsync($"api/repositories/{id}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<RepositoryModel>();
      }
      catch (HttpRequestException ex)
      {
        logger.LogError($"An error occured connecting to values API {ex.ToString()}");
        return null;
      }
    }

    public async Task CreateNewRepositoryAsync(RepositoryModel repository)
    {
      try
      {
        var response = await client.PostAsJsonAsync("api/repositories", repository);
        response.EnsureSuccessStatusCode();

        return;
      }
      catch (HttpRequestException ex)
      {
        logger.LogError($"An error occured connecting to values API {ex.ToString()}");
        return;
      }
    }

    public async Task UpdateRepositoryAsync(RepositoryModel repository)
    {
      try
      {
        var response = await client.PutAsJsonAsync($"api/repositories/{repository.Id}", repository);
        response.EnsureSuccessStatusCode();

        return;
      }
      catch (HttpRequestException ex)
      {
        logger.LogError($"An error occured connecting to values API {ex.ToString()}");
        return;
      }
    }

    public async Task DeleteRepositoryAsync(Guid id)
    {
      try
      {
        var response = await client.DeleteAsync($"api/repositories/{id}");
        response.EnsureSuccessStatusCode();

        return;
      }
      catch (HttpRequestException ex)
      {
        logger.LogError($"An error occured connecting to values API {ex.ToString()}");
        return;
      }
    }
  }
}