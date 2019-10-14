using webapi.src.contexts;
using webapi.src.models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace webapi.src.services
{
  public interface ICategoryService
  {
      Task<BaseResponse> getList();
      Task<BaseResponse> newCategory( Category data );
  }
  public class CategoryService : ICategoryService
  {
    private MyDbContext db;
    public CategoryService( MyDbContext _db ) 
    {
      db = _db ;
    }

    public async Task<BaseResponse> getList()
    {
      var list = await (
            from a in db.Categories
            select a
        ).ToListAsync();
        
        var resp = Resp.OK;
        resp.data = list;
        return resp;
    }

    public async Task<BaseResponse> newCategory( Category data )
    {
      using( var db = new MyDbContext() )
      {
        Category category = new Category{
          Title = data.Title ,
          Description = data.Description ,
          UrlSlug = data.UrlSlug ,
        };
        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();
        return Resp.OK;
      }
    }
  }
}