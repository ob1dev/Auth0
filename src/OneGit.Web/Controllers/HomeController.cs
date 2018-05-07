using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneGit.Web.Models;
using OneGit.Web.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OneGit.Web.Controllers
{
  public class HomeController : Controller
  {
    private RepositoryClient repositoryClient;

    public HomeController(RepositoryClient client)
    {
      this.repositoryClient = client;
    }

    public async Task<IActionResult> Index()
    {
      var repositories = Enumerable.Empty<RepositoryModel>();

      if (User.Identity.IsAuthenticated)
      {
        repositories = await this.repositoryClient.GetAllRepositoriesAsync();
      }

      return View(repositories);
    }

    [HttpGet]
    [Authorize(Roles = "admin, editor")]
    public IActionResult New()
    {
      return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin, editor")]
    public async Task<IActionResult> New(RepositoryModel repository)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      await this.repositoryClient.CreateNewRepositoryAsync(repository);

      return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "admin, editor")]
    public async Task<IActionResult> Edit(Guid id)
    {
      var repository = await this.repositoryClient.GetRepositoryAsync(id);

      if (repository == null)
      {
        return RedirectToAction("Index");
      }

      return View(repository);
    }

    [HttpPost]
    [Authorize(Roles = "admin, editor")]
    public async Task<IActionResult> Edit(RepositoryModel repository)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      await this.repositoryClient.UpdateRepositoryAsync(repository);

      return RedirectToAction("Index");
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
      await this.repositoryClient.DeleteRepositoryAsync(id);

      return RedirectToAction("Index");
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}