using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlaywrightTest.Core.Platform.LogisticsService;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DeliveryDetails
    {
        [JsonProperty("courierId")]
        public string CourierId { get; set; }

        [JsonProperty("courierName")]
        public string CourierName { get; set; }

        [JsonProperty("slotId")]
        public string SlotId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("deliveryStartTime")]
        public string DeliveryStartTime { get; set; }

        [JsonProperty("deliveryEndTime")]
        public string DeliveryEndTime { get; set; }

        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("zipcode")]
        public string Zipcode { get; set; }

        [JsonProperty("courierTrackingReference")]
        public string CourierTrackingReference { get; set; }

        [JsonProperty("deliveryFee")]
        public DeliveryFee DeliveryFee { get; set; }

        [JsonProperty("deliveryFeeSku")]
        public string DeliveryFeeSku { get; set; }

        [JsonProperty("instruction")]
        public string Instruction { get; set; }

        [JsonProperty("deliveredAt")]
        public string DeliveredAt { get; set; }

        [JsonProperty("landmark")]
        public string Landmark { get; set; }
    }

    public class DeliveryFee
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Price
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Result
    {
        [JsonProperty("orderReference")]
        public string OrderReference { get; set; }

        [JsonProperty("shipments")]
        public List<Shipment> Shipments { get; set; }
    }

    public class GetShipmentByOrderRefResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public class Shipment
    {
        [JsonProperty("shipmentReference")]
        public string ShipmentReference { get; set; }

        [JsonProperty("createdAt")]
        public int CreatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("substatus")]
        public string Substatus { get; set; }

        [JsonProperty("userDetails")]
        public UserDetails UserDetails { get; set; }

        [JsonProperty("deliveryDetails")]
        public DeliveryDetails DeliveryDetails { get; set; }

        [JsonProperty("shipmentItems")]
        public List<ShipmentItem> ShipmentItems { get; set; }
    }

    public class ShipmentItem
    {
        [JsonProperty("shipmentItemId")]
        public string ShipmentItemId { get; set; }

        [JsonProperty("shipmentCategory")]
        public string ShipmentCategory { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("instruction")]
        public string Instruction { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }
    }

    public class UserDetails
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("contactNumber")]
        public string ContactNumber { get; set; }
    }
