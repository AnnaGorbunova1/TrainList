namespace TrainList.Model.DTO;

public class TrainSheetItemDTO
{
    public string TrainNumber { get; set; }
    public int PositionInTrain { get; set; }
    public string InvoiceNum { get; set; }
    public string CarNumber { get; set; }
    public string FreightEtsngName { get; set; }
    public decimal FreightTotalWeightKg { get; set; }
}