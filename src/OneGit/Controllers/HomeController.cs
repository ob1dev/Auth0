using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneGit.Data;
using OneGit.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace OneGit.Controllers
{
  public class HomeController : Controller
  {
    private readonly AppDbContext appContext;

    public HomeController(AppDbContext context)
    {
      this.appContext = context;
    }

    public async Task<IActionResult> Index()
    {
      var repositoriesList = await this.appContext.Repositories.AsNoTracking().ToListAsync();
      return View(repositoriesList);
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

      this.appContext.Add(repository);

      try
      {
        await this.appContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        ViewData["Alert"] = ex.Message;
        return View();
      }

      return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "admin, editor")]
    public async Task<IActionResult> Edit(Guid id)
    {
      var repository = await this.appContext.Repositories.FindAsync(id);

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

      try
      {
        this.appContext.Attach(repository).State = EntityState.Modified;
        await this.appContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        ViewData["Alert"] = ex.Message;
        return View();
      }

      return RedirectToAction("Index");
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var repository = await this.appContext.Repositories.FindAsync(id);

      if (repository != null)
      {
        this.appContext.Remove(repository);
        await this.appContext.SaveChangesAsync();
      }

      return RedirectToAction("Index");
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}