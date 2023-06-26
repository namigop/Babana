using Newtonsoft.Json;

namespace PlaywrightTest.Platform.Rider;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class RiderUpdateStatusRequest {
    [JsonProperty("AssignedDate")] public string AssignedDate { get; set; }

    [JsonProperty("BookingDate")] public string BookingDate { get; set; }

    [JsonProperty("ConsignmentId")] public string ConsignmentId { get; set; }

    [JsonProperty("DeliveryDate")] public string DeliveryDate { get; set; }

    [JsonProperty("DestCity")] public string DestCity { get; set; }

    [JsonProperty("DestCityId")] public int DestCityId { get; set; }

    [JsonProperty("ICCID")] public string ICCID { get; set; }

    [JsonProperty("OrderInstruction")] public string OrderInstruction { get; set; }

    [JsonProperty("OrderStatus")] public string OrderStatus { get; set; }

    [JsonProperty("OrderStatusId")] public int OrderStatusId { get; set; }

    [JsonProperty("OriginCity")] public string OriginCity { get; set; }

    [JsonProperty("OriginCityId")] public int OriginCityId { get; set; }

    [JsonProperty("Reason")] public string Reason { get; set; }

    [JsonProperty("RefNum")] public int RefNum { get; set; }
}