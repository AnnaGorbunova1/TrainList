using Microsoft.EntityFrameworkCore;
using TrainList.Model;
using TrainList.Model.DTO;
using TrainList.Model.Models;

using GemBox.Spreadsheet;
using Microsoft.Office.Interop.Excel;

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

    public string CreateExcel(string trainNumber)
    {
        TrainSheet? trainSheet = _dbContext.TrainSheets
            .Include(ts => ts.Items.OrderBy(i => i.PositionInTrain))
            .FirstOrDefault(ts => ts.TrainNumber == trainNumber);
        if (trainSheet == null)
            return "";
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        var path = System.IO.Directory.GetCurrentDirectory() + Path.AltDirectorySeparatorChar +
                   "templates"+Path.AltDirectorySeparatorChar+"NL_Template.xlsx";
        
        var workbook = ExcelFile.Load(path);
        var worksheet = workbook.Worksheets.First();
        
        //заполняем заголовок
        worksheet.Cells["C3"].Value = trainNumber;
        worksheet.Cells["C4"].Value = trainSheet.SostavNumber;
        worksheet.Cells["E3"].Value = trainSheet.LastStationName;
        worksheet.Cells["E4"].Value = trainSheet.WhenLastOperation.ToShortDateString();

        var weightList = new List<Weigths>();
        int i = 7;
        decimal totalWeight = 0;
        Weigths? weightLine;
        //заполняем строки
        foreach (var item in trainSheet.Items)
        {
            worksheet.Cells["A" + i].Value = item.PositionInTrain.ToString();
            worksheet.Cells["B" + i].Value = item.CarNumber;
            worksheet.Cells["C" + i].Value = item.InvoiceNum;
            worksheet.Cells["D" + i].Value = trainSheet.WhenLastOperation.ToShortDateString();
            worksheet.Cells["E" + i].Value = item.FreightEtsngName;
            worksheet.Cells["F" + i].Value = item.FreightTotalWeightKg.ToString();
            worksheet.Cells["G" + i].Value = trainSheet.LastOperationName;
            weightLine = weightList.FirstOrDefault(ls => ls.name == item.FreightEtsngName);
            if (weightLine == null)
            {
                weightList.Add(new Weigths { name = item.FreightEtsngName, cnt = 1, weightKg = item.FreightTotalWeightKg});
            }
            else
            {
                weightLine.cnt += 1;
                weightLine.weightKg += item.FreightTotalWeightKg;
            }

            totalWeight += item.FreightTotalWeightKg;
            i++;
        }

        //заполняем футер
        int startBoldFrom = i;
        foreach (var wl in weightList)
        {
            worksheet.Cells["B" + i].Value = wl.cnt;
            worksheet.Cells["E" + i].Value = wl.name;
            worksheet.Cells["F" + i].Value = wl.weightKg;
            i++;
        }
        
        worksheet.Cells["B" + i].Value = $"Всего: {weightList.Sum(w => w.cnt)}";
        worksheet.Cells["E" + i].Value = weightList.Count;
        worksheet.Cells["F" + i].Value = weightList.Sum(w => w.weightKg);
        
        worksheet.Cells.GetSubrange("A"+startBoldFrom+":H"+i).Style.Font.Weight = ExcelFont.BoldWeight;

        var pathOut = System.IO.Directory.GetCurrentDirectory() + Path.AltDirectorySeparatorChar +
                      $"files"+Path.AltDirectorySeparatorChar+"NL_{trainNumber}.xlsx";
        workbook.Save(pathOut);
        return pathOut;
    }

    private class Weigths
    {
        public string name;
        public int cnt;
        public decimal weightKg;
    }
}