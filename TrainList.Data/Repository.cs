using Microsoft.EntityFrameworkCore;
using TrainList.Model.DTO;
using TrainList.Model.Models;

namespace TrainList.Data;

public class Repository : IRepository
{
    private readonly ApplicationContext _dbContext;

    public Repository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    private string GetSostavFromIndex(string index)
    {
        int indFrom = index.IndexOf("-");
        index = index.Substring(indFrom + 1);
        int indTo = index.IndexOf("-");
        return index.Substring(0, indTo);
    }

    private void GetHeaderFields(TrainSheetDTO trainSheetDto, ref TrainSheet trainSheet)
    {
        trainSheet.TrainNumber = trainSheetDto.TrainNumber;
        trainSheet.SostavNumber = GetSostavFromIndex(trainSheetDto.TrainIndexCombined);
        trainSheet.FromStationName = trainSheetDto.FromStationName;
        trainSheet.ToStationName = trainSheetDto.ToStationName;
        trainSheet.LastStationName = trainSheetDto.LastStationName;
        trainSheet.WhenLastOperation = DateTime.Parse(trainSheetDto.WhenLastOperation).ToUniversalTime();
        trainSheet.LastOperationName = trainSheetDto.LastOperationName;
    }

    private void GetItemFields(TrainSheetDTO trainSheetDto, ref TrainSheetItem trainSheetItem)
    {
        trainSheetItem.InvoiceNum = trainSheetDto.InvoiceNum;
        trainSheetItem.CarNumber = trainSheetDto.CarNumber;
        trainSheetItem.FreightEtsngName = trainSheetDto.FreightEtsngName;
        trainSheetItem.FreightTotalWeightKg = Decimal.Parse(trainSheetDto.FreightTotalWeightKg);
    }

    // public TrainSheet GetFirst()
    // {
    //     return _dbContext.TrainSheets.First();
    // }

    public TrainSheet GetByTrainNumber(string trainNumber)
    {
        return _dbContext.TrainSheets
            .Include(ts => ts.Items.OrderBy(i => i.PositionInTrain))
            .FirstOrDefault(ts => ts.TrainNumber == trainNumber);
    }

    public async Task CreateTrainSheets(Root trainSheetDtoContainer)
    {
        TrainSheet? trainSheet;
        TrainSheetItem? trainSheetItem;
        foreach (var trainSheetDto in trainSheetDtoContainer.TrainSheetDtos)
        {
            //ищем заголовок натурного листа
            trainSheet = await _dbContext.TrainSheets
                .FirstOrDefaultAsync(ts => ts.TrainNumber == trainSheetDto.TrainNumber);
            if (trainSheet != null)
            {
                //если нашли - обновляем
                GetHeaderFields(trainSheetDto, ref trainSheet);
                _dbContext.TrainSheets.Update(trainSheet);
            }
            else
            {
                //если не нашли, создаем
                trainSheet = new TrainSheet();
                GetHeaderFields(trainSheetDto, ref trainSheet);
                await _dbContext.TrainSheets.AddAsync(trainSheet);
            }
            await _dbContext.SaveChangesAsync();
            //ищем строку натурного листа
            trainSheetItem = await _dbContext.TrainSheetItems.FirstOrDefaultAsync(ts =>
                ts.TrainNumber == trainSheetDto.TrainNumber && ts.PositionInTrain == Int32.Parse(trainSheetDto.PositionInTrain));
            if (trainSheetItem != null)
            {
                //если нашли - обновляем
                GetItemFields(trainSheetDto, ref trainSheetItem);
                _dbContext.TrainSheetItems.Update(trainSheetItem);
            }
            else
            {
                //если не нашли - создаем
                trainSheetItem = new TrainSheetItem();
                trainSheetItem.TrainSheetId = trainSheet.Id;
                trainSheetItem.TrainNumber = trainSheet.TrainNumber;
                trainSheetItem.PositionInTrain = Int32.Parse(trainSheetDto.PositionInTrain);

                GetItemFields(trainSheetDto,ref trainSheetItem);
                await _dbContext.TrainSheetItems.AddAsync(trainSheetItem);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}