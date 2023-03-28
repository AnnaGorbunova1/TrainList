using System.ComponentModel.DataAnnotations;
namespace TrainList.Model.Models;

public class TrainSheetItem
{
    [Key]
    public int Id { get; set; }
    public int TrainSheetId { get; set; }
    public TrainSheet TrainSheet { get; set; }
    public string TrainNumber { get; set; }
    public int PositionInTrain { get; set; }
    public string InvoiceNum { get; set; }
    public string CarNumber { get; set; }
    public string FreightEtsngName { get; set; }
    public decimal FreightTotalWeightKg { get; set; }
}