using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneGit.Api.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneGit.Api.Controllers
{
  [Route("api/[controller]")]
  [Produces("application/json")]
  public class RepositoriesController : Controller
  {
    private ILogger<RepositoriesController> logger;
    private readonly RepositoryContext context;

    public RepositoriesController(ILogger<RepositoriesController> logger, RepositoryContext context)
    {
      this.logger = logger;
      this.context = context;
    }

    /// <summary>
    /// List all repositories.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/repositories
    ///     Content-Type: application/json
    ///     authorization: Bearer {your-access-token-here}   
    ///     {
    ///     }
    ///
    /// </remarks>
    /// <returns>A list of the repositories currently stored in a database.</returns>
    /// <response code="200">Returns the The specified repository.</response>
    /// <response code="401">If the header 'authorization' is not specified.</response> 
    [HttpGet]    
    [Authorize("read:repositories")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IEnumerable<RepositoryModel>> Get()
    {
      return await this.context.Repositories.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Gets the specified repository.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/repositories/12345678-90ab-cdef-1234-567890abcdef
    ///     Content-Type: application/json
    ///     authorization: Bearer {your-access-token-here} 
    ///     {
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The repository's identifier.</param>
    /// <returns>The specified repository.</returns>
    /// <response code="200">Returns the The specified repository.</response>
    /// <response code="401">If the header 'authorization' is not specified.</response> 
    /// <response code="404">If the repository hasn't been found by its identifier.</response>
    [HttpGet("{id}", Name = "GetRepository")]
    [Authorize("read:repositories")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public IActionResult Get(Guid id)
    {
      var repository = this.context.Repositories.Find(id);
      if (repository == null)
      {
        return NotFound();
      }

      return new ObjectResult(repository);
    }

    /// <summary>
    /// Creates a new repository.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST api/repositories
    ///     Content-Type: application/json
    ///     authorization: Bearer {your-access-token-here}  
    ///     {
    ///       "name": ".NET Standard",
    ///       "description": "This repo is building the .NET Standard",
    ///       "url": "https://github.com/dotnet/standard"
    ///     }
    ///
    /// </remarks>
    /// <param name="repository">The new repository.</param>
    /// <returns>A newly created repository.</returns>
    /// <response code="201">Returns the newly created repository.</response>
    /// <response code="400">If the item is null.</response> 
    /// <response code="401">If the header 'authorization' is not specified.</response> 
    [HttpPost]
    [Authorize("create:repositories")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
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

    /// <summary>
    /// Updates the specified repository.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT api/repositories/12345678-90ab-cdef-1234-567890abcdef
    ///     Content-Type: application/json
    ///     authorization: Bearer {your-access-token-here}
    ///     {
    ///       "name": "SignalR,
    ///       "description": "Incredibly simple real-time web for ASP.NET Core",
    ///       "url": "https://github.com/aspnet/SignalR"
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The repository's identifier.</param>
    /// <param name="repository">The updated repository.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the specified repository has been sucessfully deleted.</response>
    /// <response code="400">If the item is null.</response> 
    /// <response code="401">If the header 'authorization' is not specified.</response> 
    [HttpPut("{id}")]
    [Authorize("update:repositories")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
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

    /// <summary>
    /// Deletes the specified repository.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE api/repositories/12345678-90ab-cdef-1234-567890abcdef
    ///     Content-Type: application/json
    ///     authorization: Bearer {your-access-token-here}
    ///     {
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The repository's identifier.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the specified repository has been sucessfully deleted.</response>
    /// <response code="401">If the header 'authorization' is not specified.</response> 
    /// <response code="404">If the repository hasn't been found by its identifier.</response> 
    [HttpDelete("{id}")]
    [Authorize("delete:repositories")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
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