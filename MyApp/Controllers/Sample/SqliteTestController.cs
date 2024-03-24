using Microsoft.AspNetCore.Mvc;
using MyDataEF;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Models;

namespace MyApp.Controllers;

[ApiController]
[Route("sample/[controller]")]
public class SqliteTestController : ControllerBase
{
  private readonly MyDB DB;

  public SqliteTestController( MyDB _db ){ DB = _db; }

  [HttpGet]
  public async Task<IResult> Get(){

    var Projects = (from project in DB.Projects select new { ID = project.ProjectId } ).ToListAsync() ;
    var Tickets = (from ticket in DB.Tickets select new { ID = ticket.TicketId } ).ToListAsync() ;
    var DelayProjects = Task.Run(async ()=>{
      await Task.Delay(5000);
      return await (from project in DB.Projects.FromSql($"SELECT * FROM projects ORDER BY RANDOM() LIMIT 500 ") select new { ID = project.ProjectId } ).ToListAsync();
    });
    
    // run all async task at once , wait result
    await Task.WhenAll( Projects , Tickets , DelayProjects );

    var data = new { Projects = Projects.Result , Tickets = Tickets.Result , DelayProjects = DelayProjects.Result };
    
    return Util.MyApiResponse.generate( data: data ,  code:200  );
  }

  [HttpPost]
  public async Task<IResult> Post(){
    var taskInsert = Task.Run(async ()=>{
      for(var i = 0; i < 500; i++){
        DB.Projects.Add( new Project() );
        DB.Tickets.Add( new Ticket{ ProjectId = 1 } );
      }

      return await DB.SaveChangesAsync();
    });

    var taskDelete =  Task.Run( async () =>{
      await Task.Delay(3000);

      DB.Projects.RemoveRange( ( from project in DB.Projects.FromSql($"SELECT * from projects WHERE ProjectId != 1  ORDER BY RANDOM() LIMIT 217" ) select project ) );
      DB.Tickets.RemoveRange( ( from ticket in DB.Tickets.FromSql($"SELECT * from tickets ORDER BY RANDOM() LIMIT 217" ) select ticket ) );
      
      return await DB.SaveChangesAsync();
    });

    // run all async task at once , wait result
    await Task.WhenAll( taskInsert , taskDelete );
    var result = new { Inserted = taskInsert.Result , Deleted = taskDelete.Result };

    return Util.MyApiResponse.generate( code:200 , data: result );
  }

  [HttpGet("[action]")]
  public IResult TriggerGC(){
      GC.Collect();
      GC.WaitForPendingFinalizers();
    return Util.MyApiResponse.generate(); 
  }



}