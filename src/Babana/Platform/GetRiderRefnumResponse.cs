using Newtonsoft.Json;

namespace PlaywrightTest.Platform.Rider;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Result {
    [JsonProperty("ShipmentReference")] public string ShipmentReference { get; set; }

    [JsonProperty("RefNum")] public string RefNum { get; set; }
}

public class GetRiderRefnumResponse {
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("result")] public Result Result { get; set; }
}