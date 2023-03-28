using System.ComponentModel.DataAnnotations;
namespace TrainList.Model.Models;

public class TrainSheet
{
    [Key]
    public int Id { get; set; }
    public string TrainNumber { get; set; }
    public string SostavNumber { get; set; }
    public string FromStationName { get; set; }
    public string ToStationName { get; set; }
    public string LastStationName { get; set; }
    public DateTime WhenLastOperation { get; set; }
    public string LastOperationName { get; set; }
    public ICollection<TrainSheetItem> Items { get; set; }

    public TrainSheet()
    {
        Items = new List<TrainSheetItem>();
    }
}