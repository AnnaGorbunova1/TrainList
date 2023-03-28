using TrainList.Model.DTO;
using TrainList.Model.Models;

namespace TrainList.Data;

public interface IRepository
{
    // TrainSheet GetFirst();
    TrainSheet GetByTrainNumber(string trainNumber);
    Task CreateTrainSheets(Root trainSheetDtoContainer);
    string CreateExcel(string trainNumber);
}