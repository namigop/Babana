using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlaywrightTest.Core.Platform;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class APPROVAL {
    [JsonProperty("reason")] public object Reason { get; set; }

    [JsonProperty("actorId")] public string ActorId { get; set; }

    [JsonProperty("attemptCount")] public int AttemptCount { get; set; }

    [JsonProperty("updatedDate")] public DateTime UpdatedDate { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalTrackingId")] public string ExternalTrackingId { get; set; }
}

public class Entry {
    [JsonProperty("description")] public object Description { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("title")] public string Title { get; set; }
}

public class INITIALIZATION {
    [JsonProperty("reason")] public object Reason { get; set; }

    [JsonProperty("actorId")] public string ActorId { get; set; }

    [JsonProperty("attemptCount")] public int AttemptCount { get; set; }

    [JsonProperty("updatedDate")] public DateTime UpdatedDate { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalTrackingId")] public object ExternalTrackingId { get; set; }
}

public class InitialShipmentDetails {
    [JsonProperty("area")] public string Area { get; set; }

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("deliveryMethod")] public string DeliveryMethod { get; set; }

    [JsonProperty("postalCode")] public string PostalCode { get; set; }

    [JsonProperty("slot")] public Slot Slot { get; set; }

    [JsonProperty("cityInProvince")] public object CityInProvince { get; set; }

    [JsonProperty("province")] public string Province { get; set; }

    [JsonProperty("specialInstructions")] public object SpecialInstructions { get; set; }

    [JsonProperty("price")] public Price Price { get; set; }

    [JsonProperty("addressLine1")] public string AddressLine1 { get; set; }

    [JsonProperty("addressLine2")] public string AddressLine2 { get; set; }

    [JsonProperty("addressLine3")] public object AddressLine3 { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("selfPickUpLocationId")] public object SelfPickUpLocationId { get; set; }

    [JsonProperty("landmark")] public string Landmark { get; set; }
}

public class LOGISTICSFULFILLMENT {
    [JsonProperty("reason")] public object Reason { get; set; }

    [JsonProperty("actorId")] public string ActorId { get; set; }

    [JsonProperty("attemptCount")] public int AttemptCount { get; set; }

    [JsonProperty("updatedDate")] public DateTime UpdatedDate { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalTrackingId")] public object ExternalTrackingId { get; set; }
}

public class OrderBillingDetail {
    [JsonProperty("floor_no")] public string FloorNo { get; set; }

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("hse_blk_tower")] public string HseBlkTower { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("prefecture")] public string Prefecture { get; set; }

    [JsonProperty("unit_no")] public string UnitNo { get; set; }

    [JsonProperty("district")] public string District { get; set; }

    [JsonProperty("address_line_1")] public string AddressLine1 { get; set; }

    [JsonProperty("street_building_name")] public string StreetBuildingName { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("address_line_2")] public string AddressLine2 { get; set; }

    [JsonProperty("zip_code")] public string ZipCode { get; set; }
}

public class OrderDeliveryDetail {
    [JsonProperty("floor_no")] public string FloorNo { get; set; }

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("self_pick_up_location_id")]
    public object SelfPickUpLocationId { get; set; }

    [JsonProperty("hse_blk_tower")] public string HseBlkTower { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("prefecture")] public object Prefecture { get; set; }

    [JsonProperty("created_at")] public string CreatedAt { get; set; }

    [JsonProperty("delivery_time")] public object DeliveryTime { get; set; }

    [JsonProperty("delivery_note")] public object DeliveryNote { get; set; }

    [JsonProperty("zip_code")] public string ZipCode { get; set; }

    [JsonProperty("delivery_date")] public object DeliveryDate { get; set; }

    [JsonProperty("updated_at")] public string UpdatedAt { get; set; }

    [JsonProperty("unit_no")] public string UnitNo { get; set; }

    [JsonProperty("district")] public string District { get; set; }

    [JsonProperty("delivery_method")] public string DeliveryMethod { get; set; }

    [JsonProperty("address_line_1")] public string AddressLine1 { get; set; }

    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("street_building_name")] public string StreetBuildingName { get; set; }

    [JsonProperty("delivery_slot")] public string DeliverySlot { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("address_line_2")] public string AddressLine2 { get; set; }

    [JsonProperty("order_id")] public int OrderId { get; set; }
}

public class OrderUserDetail {
    [JsonProperty("identification_number")]
    public string IdentificationNumber { get; set; }

    [JsonProperty("temporary_contact_number")]
    public object TemporaryContactNumber { get; set; }

    [JsonProperty("identification_expiry_date")]
    public string IdentificationExpiryDate { get; set; }

    [JsonProperty("created_at")] public string CreatedAt { get; set; }

    [JsonProperty("last_name")] public string LastName { get; set; }

    [JsonProperty("middle_name")] public string MiddleName { get; set; }

    [JsonProperty("country_code")] public string CountryCode { get; set; }

    [JsonProperty("updated_at")] public string UpdatedAt { get; set; }

    [JsonProperty("nationality")] public string Nationality { get; set; }

    [JsonProperty("dob")] public string Dob { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("phone_number")] public string PhoneNumber { get; set; }

    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("credit_check_score")] public object CreditCheckScore { get; set; }

    [JsonProperty("order_id")] public int OrderId { get; set; }

    [JsonProperty("first_name")] public string FirstName { get; set; }

    [JsonProperty("email")] public string Email { get; set; }
}

public class PAYMENT {
    [JsonProperty("reason")] public object Reason { get; set; }

    [JsonProperty("actorId")] public string ActorId { get; set; }

    [JsonProperty("attemptCount")] public int AttemptCount { get; set; }

    [JsonProperty("updatedDate")] public DateTime UpdatedDate { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalTrackingId")] public string ExternalTrackingId { get; set; }
}

public class Plan {
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("sku")] public string Sku { get; set; }
}

public class Price {
    [JsonProperty("price")] public int Price2 { get; set; }

    [JsonProperty("currency")] public string Currency { get; set; }
}

public class Result {
    [JsonProperty("port_telco_account_number")]
    public object PortTelcoAccountNumber { get; set; }

    [JsonProperty("channel")] public string Channel { get; set; }

    [JsonProperty("order_actions")] public object OrderActions { get; set; }

    [JsonProperty("selected_number")] public string SelectedNumber { get; set; }

    [JsonProperty("number_type")] public object NumberType { get; set; }

    [JsonProperty("order_ref")] public string OrderRef { get; set; }

    [JsonProperty("authorize_enabled")] public bool AuthorizeEnabled { get; set; }

    [JsonProperty("esim_profile")] public object EsimProfile { get; set; }

    [JsonProperty("port_donor")] public object PortDonor { get; set; }

    [JsonProperty("session_token")] public string SessionToken { get; set; }

    [JsonProperty("number")] public object Number { get; set; }

    [JsonProperty("next_step")] public object NextStep { get; set; }

    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("plan")] public Plan Plan { get; set; }

    [JsonProperty("order_type")] public string OrderType { get; set; }

    [JsonProperty("order_items")] public List<object> OrderItems { get; set; }

    [JsonProperty("sim_type")] public string SimType { get; set; }

    [JsonProperty("authoriser_name")] public string AuthoriserName { get; set; }

    [JsonProperty("customer_contact_number")]
    public string CustomerContactNumber { get; set; }

    [JsonProperty("steps")] public List<Step> Steps { get; set; }

    [JsonProperty("id_upload_pending")] public bool IdUploadPending { get; set; }

    [JsonProperty("country_code")] public object CountryCode { get; set; }

    [JsonProperty("contract_type")] public string ContractType { get; set; }

    [JsonProperty("delivery_date")] public string DeliveryDate { get; set; }

    [JsonProperty("last_kyc_attempt")] public bool LastKycAttempt { get; set; }

    [JsonProperty("replacement_reason")] public object ReplacementReason { get; set; }

    [JsonProperty("port_in_request")] public bool PortInRequest { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("items")] public string Items { get; set; }

    [JsonProperty("request_type")] public object RequestType { get; set; }

    [JsonProperty("postpaid")] public bool Postpaid { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("kyc_org_code")] public object KycOrgCode { get; set; }

    [JsonProperty("order_delivery_detail")]
    public OrderDeliveryDetail OrderDeliveryDetail { get; set; }

    [JsonProperty("contact_email")] public string ContactEmail { get; set; }

    [JsonProperty("courier_name")] public object CourierName { get; set; }

    [JsonProperty("initial_shipment_details")]
    public InitialShipmentDetails InitialShipmentDetails { get; set; }

    [JsonProperty("substates")] public Substates Substates { get; set; }

    [JsonProperty("tracking_number")] public object TrackingNumber { get; set; }

    [JsonProperty("tracking_url")] public object TrackingUrl { get; set; }

    [JsonProperty("order_billing_detail")] public OrderBillingDetail OrderBillingDetail { get; set; }

    [JsonProperty("current_step")] public object CurrentStep { get; set; }

    [JsonProperty("delivery_actions")] public object DeliveryActions { get; set; }

    [JsonProperty("reset_processing")] public object ResetProcessing { get; set; }

    [JsonProperty("payment_status")] public string PaymentStatus { get; set; }

    [JsonProperty("temporary_contact_number")]
    public object TemporaryContactNumber { get; set; }

    [JsonProperty("order_user_detail")] public OrderUserDetail OrderUserDetail { get; set; }

    [JsonProperty("contact_number")] public string ContactNumber { get; set; }

    [JsonProperty("port_number")] public object PortNumber { get; set; }

    [JsonProperty("delivery_slot_name")] public object DeliverySlotName { get; set; }

    [JsonProperty("delivery_slot_full_data")]
    public string DeliverySlotFullData { get; set; }
}

public class ManageOrderResponse {
    [JsonProperty("result")] public List<Result> Result { get; set; }

    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("requestId")] public string RequestId { get; set; }
}

public class Slot {
    [JsonProperty("date")] public string Date { get; set; }

    [JsonProperty("courierName")] public string CourierName { get; set; }

    [JsonProperty("slotId")] public string SlotId { get; set; }
}

public class Step {
    [JsonProperty("entries")] public List<Entry> Entries { get; set; }

    [JsonProperty("description")] public object Description { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("title")] public string Title { get; set; }
}

public class Substates {
    [JsonProperty("PAYMENT")] public PAYMENT PAYMENT { get; set; }

    [JsonProperty("LOGISTICS_FULFILLMENT")]
    public LOGISTICSFULFILLMENT LOGISTICSFULFILLMENT { get; set; }

    [JsonProperty("TELCO_FULFILLMENT")] public TELCOFULFILLMENT TELCOFULFILLMENT { get; set; }

    [JsonProperty("APPROVAL")] public APPROVAL APPROVAL { get; set; }

    [JsonProperty("INITIALIZATION")] public INITIALIZATION INITIALIZATION { get; set; }
}

public class TELCOFULFILLMENT {
    [JsonProperty("reason")] public object Reason { get; set; }

    [JsonProperty("actorId")] public string ActorId { get; set; }

    [JsonProperty("attemptCount")] public int AttemptCount { get; set; }

    [JsonProperty("updatedDate")] public DateTime UpdatedDate { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("externalTrackingId")] public object ExternalTrackingId { get; set; }
}