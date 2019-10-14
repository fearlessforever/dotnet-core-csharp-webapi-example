using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 
using webapi.src.models;
using Microsoft.AspNetCore.Hosting;
using webapi.src.services;

namespace webapi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CategoryController : ControllerBase
  {
      private readonly ICategoryService svc;
      private readonly IHostingEnvironment env;
      public CategoryController( ICategoryService _svc )
      {
        svc = _svc; 
      }

      [HttpGet]
      public async Task<ActionResult> GetList()
      {
        var resp = await svc.getList();
        return Ok( resp );
      }

      [HttpPost]
      public async Task<ActionResult> Create([FromForm] Category data ){
        var resp = await svc.newCategory( data );
        return Ok( resp );
      }
  }
}