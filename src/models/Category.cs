using System.ComponentModel.DataAnnotations;

namespace webapi.src.models
{
  public class Category{
    public int Id{ get; set; }

    [Required]
    public string Title{ get; set; }
    public string Description{ get; set; }
    [Required]
    public string UrlSlug{ get; set; }
  }
}