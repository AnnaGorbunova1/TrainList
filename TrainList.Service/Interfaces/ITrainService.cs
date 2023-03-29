using TrainList.Model.DTO;
using TrainList.Model.Models;

namespace TrainList.Service.Interfaces;

public interface ITrainService
{
    TrainSheet GetTrainSheet(string trainNumber);
    string GetTrainSheetExcel(string trainNumber);
    Task CreateTrainSheets(Root trainSheetDtoContainer);
}