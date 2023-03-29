using TrainList.Data;
using TrainList.Model.DTO;
using TrainList.Model.Models;
using TrainList.Service.Interfaces;
using GemBox.Spreadsheet;

namespace TrainList.Service.Implementations;

public class TrainService: ITrainService
{
    private readonly IRepository _repo;

    public TrainService(IRepository repo)
    {
        _repo = repo;
    }

    public TrainSheet GetTrainSheet(string trainNumber)
    {
        return _repo.GetByTrainNumber(trainNumber);
    }

    public string GetTrainSheetExcel(string trainNumber)
    {
        TrainSheet? trainSheet = _repo.GetByTrainNumber(trainNumber);
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

    public async Task CreateTrainSheets(Root trainSheetDtoContainer)
    {
        await _repo.CreateTrainSheets(trainSheetDtoContainer);
    }
    
    private class Weigths
    {
        public string name;
        public int cnt;
        public decimal weightKg;
    }
}

