// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AdditionalIdentifiers {
    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("mobile")] public string Mobile { get; set; }
}

public class EmailLog {
    [JsonProperty("notification_id")] public string NotificationId { get; set; }

    [JsonProperty("message_id")] public string MessageId { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("logs")] public List<Log> Logs { get; set; }

    [JsonProperty("service_provider_response")]
    public ServiceProviderResponse ServiceProviderResponse { get; set; }
}

public class EventLog {
    [JsonProperty("logs")] public List<Log> Logs { get; set; }
}

public class InternalLog {
    [JsonProperty("notification_id")] public string NotificationId { get; set; }

    [JsonProperty("message_id")] public string MessageId { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("logs")] public List<Log> Logs { get; set; }

    [JsonProperty("service_provider_response")]
    public ServiceProviderResponse ServiceProviderResponse { get; set; }
}

public class Log {
    [JsonProperty("raw_id")] public string RawId { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("mobile")] public string Mobile { get; set; }

    [JsonProperty("isd_code")] public string IsdCode { get; set; }

    [JsonProperty("user_id")] public string UserId { get; set; }

    [JsonProperty("activity_id")] public string ActivityId { get; set; }

    [JsonProperty("entry_gate")] public string EntryGate { get; set; }

    [JsonProperty("team_key")] public string TeamKey { get; set; }

    [JsonProperty("request_data")] public RequestData RequestData { get; set; }

    [JsonProperty("tenant")] public string Tenant { get; set; }

    [JsonProperty("channels")] public List<string> Channels { get; set; }

    [JsonProperty("event_log")] public EventLog EventLog { get; set; }

    [JsonProperty("sms_log")] public SmsLog SmsLog { get; set; }

    [JsonProperty("email_log")] public EmailLog EmailLog { get; set; }

    [JsonProperty("internal_log")] public InternalLog InternalLog { get; set; }

    [JsonProperty("created_at")] public long CreatedAt { get; set; }

    [JsonProperty("created_date_time")] public DateTime CreatedDateTime { get; set; }

    [JsonProperty("timestamp")] public object Timestamp { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("log_message")] public string LogMessage { get; set; }
}

public class MockAcceptsNotification {
    [JsonProperty("latency")] public int Latency { get; set; }

    [JsonProperty("server-code")] public int ServerCode { get; set; }
}

public class Payload {
    [JsonProperty("auth_id")] public string AuthId { get; set; }

    [JsonProperty("otp_code")] public string OtpCode { get; set; }

    [JsonProperty("pin")] public string Pin { get; set; }

    [JsonProperty("user_id")] public string UserId { get; set; }
}

public class RequestData {
    [JsonProperty("activity_id")] public string ActivityId { get; set; }

    [JsonProperty("additional_identifiers")]
    public AdditionalIdentifiers AdditionalIdentifiers { get; set; }

    [JsonProperty("attachments")] public List<object> Attachments { get; set; }

    [JsonProperty("client_id")] public string ClientId { get; set; }

    [JsonProperty("identifier_key")] public string IdentifierKey { get; set; }

    [JsonProperty("identifier_value")] public string IdentifierValue { get; set; }

    [JsonProperty("locale")] public string Locale { get; set; }

    [JsonProperty("mock_accepts_notification")]
    public MockAcceptsNotification MockAcceptsNotification { get; set; }

    [JsonProperty("mock_auth")] public bool MockAuth { get; set; }

    [JsonProperty("payload")] public Payload Payload { get; set; }

    [JsonProperty("priority")] public string Priority { get; set; }

    [JsonProperty("sms_sender_number")] public string SmsSenderNumber { get; set; }
}

public class NotificationRoot {
    [JsonProperty("count")] public int Count { get; set; }

    [JsonProperty("logs")] public List<Log> Logs { get; set; }
}

public class ServiceProviderResponse {
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("response")] public object Response { get; set; }

    [JsonProperty("provider_tried")] public object ProviderTried { get; set; }
}

public class SmsLog {
    [JsonProperty("notification_id")] public string NotificationId { get; set; }

    [JsonProperty("message_id")] public string MessageId { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("logs")] public List<Log> Logs { get; set; }

    [JsonProperty("service_provider_response")]
    public ServiceProviderResponse ServiceProviderResponse { get; set; }
}