using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlaywrightTest.Platform.Inventory;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class ExternalRule {
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("externalVal")] public string ExternalVal { get; set; }

    [JsonProperty("externalType")] public string ExternalType { get; set; }
}

public class Price {
    [JsonProperty("value")] public int Value { get; set; }

    [JsonProperty("currency")] public string Currency { get; set; }

    [JsonProperty("prefix")] public string Prefix { get; set; }
}

public class ProductVariantIdentifier {
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("identifierKey1")] public string IdentifierKey1 { get; set; }

    [JsonProperty("identifierValue1")] public string IdentifierValue1 { get; set; }

    [JsonProperty("identifierKey2")] public string IdentifierKey2 { get; set; }

    [JsonProperty("identifierValue2")] public string IdentifierValue2 { get; set; }

    [JsonProperty("identifierKey3")] public string IdentifierKey3 { get; set; }

    [JsonProperty("identifierValue3")] public string IdentifierValue3 { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalKey")] public string ExternalKey { get; set; }

    [JsonProperty("productVariantSku")] public string ProductVariantSku { get; set; }

    [JsonProperty("expireAt")] public int ExpireAt { get; set; }

    [JsonProperty("enabledAt")] public int EnabledAt { get; set; }

    [JsonProperty("createdAt")] public int CreatedAt { get; set; }

    [JsonProperty("price")] public Price Price { get; set; }

    [JsonProperty("productCategory")] public string ProductCategory { get; set; }

    [JsonProperty("inventoryCategory")] public string InventoryCategory { get; set; }

    [JsonProperty("externalRules")] public List<ExternalRule> ExternalRules { get; set; }
}

public class Result {
    [JsonProperty("productVariantIdentifiers")]
    public List<ProductVariantIdentifier> ProductVariantIdentifiers { get; set; }
}

public class GetIccIdsResponse {
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("result")] public Result Result { get; set; }
}