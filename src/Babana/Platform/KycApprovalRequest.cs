using Newtonsoft.Json;

namespace PlaywrightTest.Core.Platform.KycService;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class CustomerKyc {
    [JsonProperty("CurrentAddress")] public string CurrentAddress { get; set; }

    [JsonProperty("EName")] public string EName { get; set; }

    [JsonProperty("EPermanentAddress")] public string EPermanentAddress { get; set; }

    [JsonProperty("EFatherSpouseName")] public string EFatherSpouseName { get; set; }

    [JsonProperty("PermanentAddress")] public string PermanentAddress { get; set; }

    [JsonProperty("MotherName")] public string MotherName { get; set; }

    [JsonProperty("FatherSpouseName")] public string FatherSpouseName { get; set; }

    [JsonProperty("EmotherName")] public string EmotherName { get; set; }

    [JsonProperty("OldageFlag")] public string OldageFlag { get; set; }

    [JsonProperty("ECurrentAddress")] public string ECurrentAddress { get; set; }

    [JsonProperty("Name")] public string Name { get; set; }

    [JsonProperty("PlaceOfBirth")] public string PlaceOfBirth { get; set; }
}

public class KycApprovalRequest {
    [JsonProperty("Msisdn")] public string Msisdn { get; set; }

    [JsonProperty("DeviceId")] public string DeviceId { get; set; }

    [JsonProperty("ErrorDesc")] public string ErrorDesc { get; set; }

    [JsonProperty("PmdId")] public string PmdId { get; set; }

    [JsonProperty("CNIC")] public string CNIC { get; set; }

    [JsonProperty("Pmdtype")] public string Pmdtype { get; set; }

    [JsonProperty("OrderId")] public string OrderId { get; set; }

    [JsonProperty("StatusCode")] public string StatusCode { get; set; }

    [JsonProperty("BioType")] public string BioType { get; set; }

    [JsonProperty("CustomerKyc")] public CustomerKyc CustomerKyc { get; set; }

    [JsonProperty("RetailId")] public string RetailId { get; set; }
}