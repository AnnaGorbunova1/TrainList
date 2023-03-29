using Microsoft.AspNetCore.Mvc;
using TrainList.Model.DTO;
using TrainList.Service.Interfaces;

namespace TrainList.Controllers;

[ApiController]
[Route("[controller]")]
public class TrainSheetController : ControllerBase
{
    private readonly ITrainService _service;

    public TrainSheetController(ITrainService service)
    {
        _service = service;
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
            string file_path = _service.GetTrainSheetExcel(trainNumber);
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
        return new ObjectResult(_service.GetTrainSheet(trainNumber));
    }

    [HttpPost(Name = "PostTrainSheets")]
    [Consumes("application/xml")]
    public async Task<ActionResult> Post(Root testXmlModel)
    {
        await _service.CreateTrainSheets(testXmlModel);
        return CreatedAtAction("Post",new {processed = testXmlModel.TrainSheetDtos.Count});
    }
}