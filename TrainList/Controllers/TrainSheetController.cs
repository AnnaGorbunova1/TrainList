using Microsoft.AspNetCore.Mvc;
using TrainList.Data;
using TrainList.Model.DTO;
using TrainList.Model.Models;

namespace TrainList.Controllers;

[ApiController]
[Route("[controller]")]
public class TrainSheetController : ControllerBase
{
    private readonly IRepository _repo;

    public TrainSheetController(IRepository repo)
    {
        _repo = repo;
    }

    // [HttpGet(Name = "GetFirstTrainSheet")]
    // public TrainSheet Get()
    // {
    //     return _repo.GetFirst();
    // }
    
    [HttpGet("{trainNumber}", Name = "GetTrainSheetExcel")]
    public ActionResult GetTrainSheetExcel(string trainNumber,bool isExcel)
    {
        if (isExcel)
        {
            string file_path = _repo.CreateExcel(trainNumber);
            // Тип файла - content-type
            string file_type = "application/xlsx";
            // Имя файла 
            string file_name = $"NL_{trainNumber}.xlsx";
            if (string.IsNullOrEmpty(file_path))
            {
                return NotFound();
            }
            return PhysicalFile(file_path, file_type, file_name);
        }
        return new ObjectResult(_repo.GetByTrainNumber(trainNumber));
    }

    [HttpPost(Name = "PostTrainSheets")]
    [Consumes("application/xml")]
    public async Task<ActionResult> Post(Root testXmlModel)
    {
        await _repo.CreateTrainSheets(testXmlModel);
        return CreatedAtAction("Post",new {processed = testXmlModel.TrainSheetDtos.Count});
    }
}