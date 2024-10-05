using System.Text.Json.Serialization;

public class BiomarkerData
{
    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("periodicity")]
    public string Periodicity { get; set; }

    [JsonPropertyName("aggregation")]
    public string Aggregation { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; } 

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("valueType")]
    public string ValueType { get; set; }

    [JsonPropertyName("startDateTime")]
    public DateTime StartDateTime { get; set; }

    [JsonPropertyName("endDateTime")]
    public DateTime EndDateTime { get; set; }
}
