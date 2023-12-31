using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
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
    public static async Task<string> GetPortinOtp(string url, string mobile, Cancel cancel = null) {
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

    public static async Task<CookieContainer> GenerateToken(string url, string username, string password) {
        // Create Handler
        var handler = new HttpClientHandler();

        // Cookies
        var cc = new CookieContainer();
        handler.CookieContainer = cc;
        using var client = new HttpClient(handler);

        // Form Data clientId=cl&clientSecret=secret&username=erik.araojo%40circles.asia&grantType=password&password=Z%40q12wsxzaq1
        var dict4 = new Dictionary<string, string> {
            { "clientId", "cl" },
            { "clientSecret", "secret" },
            { "username", username },
            { "grantType", "password" },
            { "password", password },
        }; // dict

        using var response4 = await client.PostAsync(url, new FormUrlEncodedContent(dict4));
        response4.EnsureSuccessStatusCode();
        string responseBody4 = await response4.Content.ReadAsStringAsync();
        return cc;
    }

    public static async Task<string> GetOtp(string url, string email, CookieContainer cookies, Cancel cancel = null) {
        cancel?.TryCancel();

        var handler = new HttpClientHandler() {
            CookieContainer = cookies
        };

        using var client = new HttpClient(handler);

        var sw = Stopwatch.StartNew();
        var msg = new HttpRequestMessage(HttpMethod.Get, url);

        //msg.Headers.Add("Cookie", "cmsuser=erik.araojo%40circles.asia; cmscookie=7e7350fb2c2b1e4a14a2795eedafc5b8%7Cerik.araojo%40circles.asia");
        using var response = await client.SendAsync(msg);
        sw.Stop();
        response.EnsureSuccessStatusCode();
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

    public static async Task ShipmentUpdate(string riderUrl, string status, int statusId, string iccid, string refNum, Cancel cancel = null) {
        cancel?.TryCancel();

        var today = DateTime.Today.ToString("yyyy-MM-dd");
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
        response.EnsureSuccessStatusCode();
        var uri = new Uri(riderUrl);
        var reqMethod = "POST";
        var reqBody = Util.Serialize(req);
        var respBody = await response.Content?.ReadAsStringAsync();

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);
    }

    public static async Task DoKyc(string kycUrl, string kycStatus, string orderRef, string number, string cnic, Cancel cancel = null, int tries = 0) {
        cancel?.TryCancel();
        if (tries > 2) return;

        await Task.Delay(500);
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

        try {
            using var response = await client.PostAsJsonAsync(kycUrl, kycReq);
            sw.Stop();

            var uri = new Uri(kycUrl);
            var reqMethod = "POST";
            var reqBody = Util.Serialize(kycReq);
            var respBody = await response.Content?.ReadAsStringAsync();

            ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


            if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Unable to perform KYC for {orderRef} to status {kycStatus}. {(int)response.StatusCode} {response.StatusCode}");
        }
        catch (HttpRequestException exc) {
            //the kyc server sucks balls. flaky.
            await DoKyc(kycUrl, kycStatus, orderRef, number, cnic, cancel, tries + 1);
        }
    }

    public static async Task<string> GetShipmentRef(string laasUrl, string orderRef, Cancel cancel = null) {
        cancel?.TryCancel();

        //http://qpkvc-logistics-singpost.onic.com.pk/v1/internal/shipments?orderReference=CXO-AMN4BN33QB

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Service-ID", "CXOMS");
        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(laasUrl);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<GetShipmentByOrderRefResponse>(responseContent);
        sw.Stop();

        var uri = new Uri(laasUrl);
        var reqMethod = "GET";
        var reqBody = "";
        var respBody = responseContent;

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (root.Success) {
            return root.Result.Shipments[0].ShipmentReference;
        }

        throw new Exception($"Unable to retrieve the shipment reference for order {orderRef}. {(int)response.StatusCode} {response.StatusCode}");
    }

    public static async Task<string> GetIccid(string imsUrl, Cancel cancel = null) {
        cancel?.TryCancel();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Service-ID", "CXOMS");

        var sw = Stopwatch.StartNew();
        using var response = await client.GetAsync(imsUrl);
        response.EnsureSuccessStatusCode();
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

    public static async Task<string> GetRefNum(string riderUrl, string shipmentRef, string apiKey, Cancel cancel = null) {
        // http://qpkvc-logistics-riders.onic.com.pk/v1/internal/shipments/{{shipmentReference}}/refnum
        cancel?.TryCancel();
        using var client = new HttpClient();

        var sw = Stopwatch.StartNew();
        client.DefaultRequestHeaders.Add("Api-key", apiKey);
        using var response = await client.GetAsync(riderUrl);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<GetRiderRefnumResponse>(responseContent);

        //silgpm4hqi0nuwfsdkr97ldaw5jq2ghm
        var uri = new Uri(riderUrl);
        var reqMethod = "GET";
        var reqBody = "";
        var respBody = responseContent;

        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, response.RequestMessage.Headers, response.Headers, response.StatusCode, sw.ElapsedMilliseconds);


        if (root.Success) return root.Result.RefNum;

        throw new Exception($"Unable to retrieve the Refnum reference for ShipmentRef {shipmentRef}. {(int)response.StatusCode} {response.StatusCode}");
    }
}
