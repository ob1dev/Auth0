using System;
using System.ComponentModel.DataAnnotations;

namespace Auth0
{
  public class RepositoryModel
  {
    public Guid ID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [StringLength(256)]
    public string Description { get; set; }

    [StringLength(100)]
    public string Url { get; set; }
  }
}