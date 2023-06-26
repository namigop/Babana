using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlaywrightTest.Core;
using PlaywrightTest.Core.Platform.KycService;
using PlaywrightTest.Core.Platform.LogisticsService;
using PlaywrightTest.Models;
using PlaywrightTest.Platform.Inventory;
using PlaywrightTest.Platform.Rider;

namespace PlaywrightTest.ScriptingExtensions;

public static class PlatformFunctions {
    public static async Task<string> getPortinOtp(string url, string mobile, Cancel cancel = null) {
        cancel?.TryCancel();
        using var client = new HttpClient();
        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(url);
        sw.Stop();
        var responseContent = await response.Content.ReadAsStringAsync();
        ReqRespTracer.Trace(new Uri(url), "GET", "", responseContent, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);

        var root = JsonConvert.DeserializeObject<NotificationRoot>(responseContent);
        foreach (var l in root?.Logs)
            if (mobile == l.Mobile && l.RequestData.ActivityId == "mnp_number_check") {
                Console.WriteLine($"Found OTP {l.RequestData.Payload.OtpCode} for {mobile}");
                return l.RequestData.Payload.OtpCode;
            }

        throw new Exception("MNP : OTP not found");
    }

    public static async Task<string> getOtp(string url, string email, Cancel cancel = null) {
        cancel?.TryCancel();

        using var client = new HttpClient();
        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(url);
        sw.Stop();
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<NotificationRoot>(responseContent);

        ReqRespTracer.Trace(new Uri(url), "GET", "", responseContent, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);

        foreach (var l in root?.Logs)
            if (email.ToLowerInvariant() == l.Email.ToLowerInvariant() && l.RequestData.ActivityId == "activate_app") {
                Console.WriteLine($"Found OTP {l.RequestData.Payload.OtpCode} for {l.Email}");
                return l.RequestData.Payload.OtpCode;
            }

        throw new Exception("Activate App : OTP not found");
    }

    public static async Task shipmentUpdate(string riderUrl, string status, int statusId, string iccid, string refNum, Cancel cancel = null) {
        cancel?.TryCancel();

        var today = "";
        var req = new RiderUpdateStatusRequest() {
            AssignedDate = today,
            BookingDate = today,
            ConsignmentId = "consignment-id-not-1",
            DeliveryDate = today,
            DestCity = "Karachi",
            DestCityId = 1,
            ICCID = iccid,
            OrderInstruction = "Order Instruction",
            OrderStatus = status,
            OrderStatusId = statusId,
            OriginCity = "Karachi",
            OriginCityId = 1,
            RefNum = Convert.ToInt32(refNum),
            Reason = "Sim delivery"
        };

        using var client = new HttpClient();
        var sw = Stopwatch.StartNew();
        using var response = await client.PostAsJsonAsync(riderUrl, req);
        sw.Stop();
        var uri = new Uri(riderUrl);
        var reqMethod = "POST";
        var reqBody = Util.Serialize(req);
        var respBody = await response.Content?.ReadAsStringAsync();

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);
    }

    public static async Task doKyc(string kycUrl, string kycStatus, string orderRef, string number, string cnic, Cancel cancel = null) {
        cancel?.TryCancel();

        var kycReq = new KycApprovalRequest() {
            Msisdn = number,
            DeviceId = Guid.NewGuid().ToString(),
            PmdId = Guid.NewGuid().ToString(),
            RetailId = Guid.NewGuid().ToString(),
            CNIC = cnic,
            OrderId = number,
            StatusCode = kycStatus,
            BioType = "1",
            CustomerKyc = new CustomerKyc()
        };

        using var client = new HttpClient();
        var sw = Stopwatch.StartNew();
        using var response = await client.PostAsJsonAsync(kycUrl, kycReq);
        sw.Stop();

        var uri = new Uri(kycUrl);
        var reqMethod = "POST";
        var reqBody = Util.Serialize(kycReq);
        var respBody = await response.Content?.ReadAsStringAsync();

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Unable to perform KYC for {orderRef} to status {kycStatus}. {(int)response.StatusCode} {response.StatusCode}");
    }

    public static async Task<string> getShipmentRef(string laasUrl, string orderRef, Cancel cancel = null) {
        cancel?.TryCancel();

        //http://qpkvc-logistics-singpost.onic.com.pk/v1/internal/shipments?orderReference=CXO-AMN4BN33QB

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Service-ID", "CXOMS");
        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(laasUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<GetShipmentByOrderRefResponse>(responseContent);
        sw.Stop();

        var uri = new Uri(laasUrl);
        var reqMethod = "GET";
        var reqBody = "";
        var respBody = responseContent;

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (root.Success) return root.Result.Shipments[0].ShipmentReference;

        throw new Exception($"Unable to retrieve the shipment reference for order {orderRef}. {(int)response.StatusCode} {response.StatusCode}");
    }

    public static async Task<string> getIccid(string imsUrl, Cancel cancel = null) {
        cancel?.TryCancel();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Service-ID", "CXOMS");

        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(imsUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<GetIccIdsResponse>(responseContent);

        var uri = new Uri(imsUrl);
        var reqMethod = "GET";
        var reqBody = "";
        var respBody = responseContent;

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (root.Success && root.Result.ProductVariantIdentifiers.Any())
            return root.Result.ProductVariantIdentifiers
                .First(i => i.IdentifierKey1 == "iccid")
                .IdentifierValue1;

        throw new Exception($"Unable to retrieve the ICCId. {(int)response.StatusCode} {response.StatusCode}");
    }

    public static async Task<string> getRefNum(string riderUrl, string shipmentRef, Cancel cancel = null) {
        // http://qpkvc-logistics-riders.onic.com.pk/v1/internal/shipments/{{shipmentReference}}/refnum
        cancel?.TryCancel();
        using var client = new HttpClient();

        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(riderUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<GetRiderRefnumResponse>(responseContent);

        var uri = new Uri(riderUrl);
        var reqMethod = "GET";
        var reqBody = "";
        var respBody = responseContent;

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (root.Success) return root.Result.RefNum;

        throw new Exception($"Unable to retrieve the Refnum reference for ShipmentRef {shipmentRef}. {(int)response.StatusCode} {response.StatusCode}");
    }
}
