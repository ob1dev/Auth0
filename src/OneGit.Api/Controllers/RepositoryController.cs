using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneGit.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneGit.Api.Controllers
{
  [Route("api/[controller]")]
  public class RepositoryController : Controller
  {
    private ILogger<RepositoryController> logger;
    private readonly RepositoryDbContext context;

    public RepositoryController(ILogger<RepositoryController> logger, RepositoryDbContext context)
    {
      this.logger = logger;
      this.context = context;
    }

    // GET api/repository
    [HttpGet]    
    public async Task<IEnumerable<RepositoryModel>> Get()
    {
      return await this.context.Repositories.AsNoTracking().ToListAsync();
    }

    // GET api/repository/12345678-90ab-cdef-1234-567890abcdef
    [HttpGet("{id}")]
    [HttpGet("{id}", Name = "GetRepository")]
    public IActionResult Get(Guid id)
    {
      var item = this.context.Repositories.FirstOrDefault(repository => repository.Id == id);
      if (item == null)
      {
        return NotFound();
      }

      return new ObjectResult(item);
    }

    // POST api/repository
    [HttpPost]
    public IActionResult Post([FromBody] RepositoryModel repository)
    {
      if (repository == null)
      {
        return BadRequest();
      }

      try
      {

        this.context.Repositories.Add(repository);
        this.context.SaveChanges();
      }
      catch (Exception ex)
      {
        this.logger.LogError($"An error occurred while creating entity '{nameof(RepositoryModel)}': {ex.ToString()}");
      }

      return CreatedAtRoute("GetRepository", new { id = repository.Id }, repository);

    }

    // PUT api/repository/12345678-90ab-cdef-1234-567890abcdef
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] RepositoryModel repository)
    {
      if (repository == null || repository.Id != id)
      {
        return BadRequest();
      }

      try
      {
        this.context.Attach(repository).State = EntityState.Modified;
        await this.context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        this.logger.LogError($"An error occurred while updating the entity '{nameof(RepositoryModel)}' with id {id}: {ex.ToString()}");
      }

      return new NoContentResult();
    }

    // DELETE api/repository/12345678-90ab-cdef-1234-567890abcdef
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var repository = await this.context.Repositories.FindAsync(id);
      if (repository == null)
      {
        return NotFound();
      }

      try
      {
        this.context.Repositories.Remove(repository);
        await this.context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        this.logger.LogError($"An error occurred while deleting the entity '{nameof(RepositoryModel)}' with id '{id}': {ex.ToString()}");
      }

      return new NoContentResult();
    }
  }
}