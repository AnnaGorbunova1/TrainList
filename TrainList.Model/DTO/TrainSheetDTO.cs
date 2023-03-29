using System.Xml.Serialization;

namespace TrainList.Model.DTO;

// [System.SerializableAttribute()]
// [XmlRoot(ElementName = "Root")]
// // [DataContract(Name = "Root")]
// public class ContainerDTO 
// {
//     [XmlElement(ElementName = "row")]
//     // [DataMember(Name = "row")]
//     public IEnumerable<TrainSheetDTO> TrainSheetDtos { get; set; } = new List<TrainSheetDTO>();
// }

public class TrainSheetDTO
{
    [XmlElement(ElementName = "TrainNumber")]
    public string TrainNumber { get; set; }
    [XmlElement(ElementName = "TrainIndexCombined")]
    public string TrainIndexCombined { get; set; }
    [XmlElement(ElementName = "FromStationName")]
    public string FromStationName { get; set; }
    [XmlElement(ElementName = "ToStationName")]
    public string ToStationName { get; set; }
    [XmlElement(ElementName = "LastStationName")]
    public string LastStationName { get; set; }
    [XmlElement(ElementName = "WhenLastOperation")]
    public string WhenLastOperation { get; set; }
    [XmlElement(ElementName = "LastOperationName")]
    public string LastOperationName { get; set; }
    [XmlElement(ElementName = "InvoiceNum")]
    public string InvoiceNum { get; set; }
    [XmlElement(ElementName = "PositionInTrain")]
    public string PositionInTrain { get; set; }
    [XmlElement(ElementName = "CarNumber")]
    public string CarNumber { get; set; }
    [XmlElement(ElementName = "FreightEtsngName")]
    public string FreightEtsngName { get; set; }
    [XmlElement(ElementName = "FreightTotalWeightKg")]
    public string FreightTotalWeightKg { get; set; }
    
}

[XmlRoot]
public class Root
{
    [XmlElement(ElementName = "row")]
    public List<TrainSheetDTO> TrainSheetDtos { get; set; }
}

// public class SomeRow
// {
//     [XmlElement(ElementName = "Name")]
//     public string Name { get; set; }
//     [XmlElement(ElementName = "Number")]
//     public string Number { get; set; } 
// }