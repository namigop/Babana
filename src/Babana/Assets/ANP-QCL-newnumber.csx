
//********************************************
// This is a script executes the New Number flow
// automatically doing KYC, and sim activation
//********************************************

//modify these settings to match your screen resolution
var BROWSER_HEIGHT = 880;
var BROWSER_WIDTH = 1512;
var HEADLESS = true;
var SLOMO_MSEC = 300;

var r = new Random();
var email = $"t{Environment.UserName}{r.Next(50, 100000)}@bar.com";
var deliveryDate = "18/07/2023";
var deliveryTime = "10:00 AM - 10:00 PM"; 
var plan = "OCTFVbzpGi";
var cnic = getRandomCnic();
var contactNumber = getRandomMobile("03");
var nationality = "pakistani";

var firstName = "Foo";
var middleName = "Bar";
var lastName = "Baz";

var dobDay = "31";
var dobMonth = "May";
var dobYear = "1990";

var cnic_dobDay = "25";
var cnic_dobMonth = "Dec";
var cnic_dobYear = "2030";

var address1 = "22 Henderson Road";
var address2 = "Ugly building";
var province = "Federal Capital";
var city = "Islamabad";
var area = "BAGNIAL";
var landmark = "Red door with a golden door knob";
var deliveryInstruction = "Ring the doorbell and sing";

var kycStatus = "109";
// ---------------------------------

//--- Constants ------------------
var CHECKOUT_URL = "https://qcl-webfrontek.cxos.tech/web/pre-checkout?reset=true"; 
var OTP_URL = "http://qcl-notify.cxos.tech/api/v1/unified_ui/notification/admin/logs?idisplayStart=0&idisplayLength=10";
var KYC_URL = "http://qcl-kyc-bvs.cxos.tech/mno/conn/v1/kyc/notif";
var LAAS_URL = "http://qcl-logistics-singpost.cxos.tech/v1/internal/shipments?orderReference={{orderRef}}";
var RIDER_URL = "http://qcl-logistics-riders.cxos.tech/v1/internal/shipments/{{shipmentReference}}/refnum";
var IMS_URL = "http://qcl-inventory.cxos.tech/v2/internal/product-variant-identifiers?pviType=physicalSim&limit=10&page=1&status=available";
var RIDER_NOTIF_URL = "http://qcl-logistics-riders.cxos.tech/v1/external/notification";

var NEXT ="Next";
var CONTINUE = "Continue";


var OK = "OK";
var PLACE_ORDER = "Place order";

var PLACEHOLDER_FIRSTNAME = "First name";
var PLACEHOLDER_MIDDLENAME = "Middle name";
var PLACEHOLDER_LASTNAME = "Last name";
var PLACEHOLDER_DAY = "Day";
var PLACEHOLDER_MONTH = "Month";
var PLACEHOLDER_YEAR = "Year";
var PLACEHOLDER_CNIC = "1234512345671";

var TEST_ID_PLANCARD = "planCard";
var TEST_ID_CONTACT_NUMBER = "contact_number";
var TEST_ID_NATIONALITY = "nationality";
var TEST_ID_CNIC = "identification_number";
var TEST_ID_CNIC_RECONFIRM = "reConfirmidentificationNumber";
var TEST_ID_DOB_DAY = "dobDay";   
var TEST_ID_DOB_MONTH = "dobMonth";  
var TEST_ID_DOB_YEAR = "dobYear"; 
var TEST_ID_CNIC_DOB_DAY = "document_dobDay";   
var TEST_ID_CNIC_DOB_MONTH = "document_dobMonth";  
var TEST_ID_CNIC_DOB_YEAR = "document_dobYear"; 
var TEST_ID_ADDRESS1 = "delivery_address_line_1";
var TEST_ID_ADDRESS2 = "delivery_address_line_2";
var TEST_ID_PROVINCE = "delivery_province";
var TEST_ID_CITY = "delivery_city"; 
var TEST_ID_AREA = "delivery_area"; 
var TEST_ID_ZIPCODE = "delivery_zip_code"; 
var TEST_ID_LANDMARK = "delivery_landmark"; 
var TEST_ID_DELIVERY_INSTRUCTION = "delivery_instructions"; 
var TEST_ID_DELIVERY_SLOTS = "delivery_date,delivery_slot_full_data";

//---- start of test execution ---------

Print("Test is starting...");
TestEnv.TestOrder.Email = email;
var run = Setup()
          .Slomo(SLOMO_MSEC)
          .Headless(HEADLESS)
          .BrowserHeight(BROWSER_HEIGHT)
          .BrowserWidth(BROWSER_WIDTH)
          .Chromium();

var page = await run.Begin(TestEnv);



//1. Checkout Page
await page.Open(CHECKOUT_URL);
await page.FindById(TEST_ID_PLANCARD)
          .FilterByText(page, plan)
          .FindButton()
          .Click();
          

return;

await Sleep(1000);

//2. Addon Page
await page.WaitFor("http.*/addon-selection");
await page.FindButton()
          .FilterByText(page, NEXT)
          .Click();

//3. Signup page
await page.FindTextBox()
          .Fill(email);
          
await page.FindById("agreed_to_terms")
          .FindByText("I agree to the ")
          .First
          .Click();
await page.MouseWheel(0, 1000);
await page.FindById("terms-privacy-button").Click();

await page.FindById("agreed_to_policy")
          .FindByText("I agree to the ")
          .First
          .Click();
await page.MouseWheel(0, 1000);
await page.FindById("terms-privacy-button").Click();


await page.FindButton()
          .FilterByText(page, CONTINUE)
          .Click();

//4. OTP page
await Sleep(300);
var otp = await GetOtp(OTP_URL, email);
page.FindTextBox()
    .FillMany(otp);
await page.FindButton()
          .FilterByText(page, NEXT)
          .Click();


//5. New Number page
await Sleep(1000);
await page.FindByText("Standard").Click();
await page.FindById("number-select-button").First.Click();
await page.FindButton()
          .FilterByText(page, NEXT)
          .Click();


//6. Personal Details page
await page.WaitFor("http.*web/personal-details", CancelToken);
await page.FindByPlaceholder(PLACEHOLDER_FIRSTNAME).Fill(firstName);
await page.FindByPlaceholder(PLACEHOLDER_MIDDLENAME).Fill(middleName);
await page.FindByPlaceholder(PLACEHOLDER_LASTNAME).Fill(lastName);

await page.FindById(TEST_ID_DOB_DAY).FindByText(PLACEHOLDER_DAY).Click();
await page.Keyboard(dobDay);
await page.FindById(TEST_ID_DOB_MONTH).FindByText(PLACEHOLDER_MONTH).Click();
await page.Keyboard(dobMonth);
await page.FindById(TEST_ID_DOB_YEAR).FindByText(PLACEHOLDER_YEAR).Click();
await page.Keyboard(dobYear);
 
await page.FindById(TEST_ID_CONTACT_NUMBER).FindTextBox().Fill(contactNumber);   
await page.FindById(TEST_ID_NATIONALITY).Click();
await page.Keyboard(nationality);   
await page.FindById(TEST_ID_CNIC).FindTextBox().Fill(cnic);
//await page.FindById(TEST_ID_CNIC_RECONFIRM).FindTextBox().Fill(cnic);

await page.FindById(TEST_ID_CNIC_DOB_DAY).Click();
await page.Keyboard(cnic_dobDay);   
await page.FindById(TEST_ID_CNIC_DOB_MONTH).Click();
await page.Keyboard(cnic_dobMonth);   
await page.FindById(TEST_ID_CNIC_DOB_YEAR).Click();
await page.Keyboard(cnic_dobYear);   

await page.FindById(TEST_ID_ADDRESS1).FindTextBox().Fill(address1);
await page.FindById(TEST_ID_ADDRESS2).FindTextBox().Fill(address2);
await page.FindById(TEST_ID_PROVINCE).Click();
await page.Keyboard(province);
await page.FindById(TEST_ID_CITY).Click();
await page.Keyboard(city);
await page.FindById(TEST_ID_AREA).Click();
await page.Keyboard(area);
await page.FindById(TEST_ID_LANDMARK).FindTextBox().Fill(landmark);
await page.FindById(TEST_ID_DELIVERY_INSTRUCTION).FindTextBox().Fill(deliveryInstruction);
await page.FindById(TEST_ID_DELIVERY_SLOTS)
          .FindButton()
          .FilterByText(page, deliveryDate)
          .Click();

await page.FindById(TEST_ID_DELIVERY_SLOTS)
          .FindButton()
          .FilterByText(page, deliveryTime)
          .Click();
await page.FindButton().FilterByText(page, NEXT).Click();

//7. Order Summary Page
await page.WaitFor("http.*web/order-summary", CancelToken);
await page.FindButton().FilterByText(page, PLACE_ORDER).Click();

//8. stripe Page
await page.WaitFor("http.*web/payment", CancelToken);
var frame = page.FindFrame("stripe.com").First;
await frame.FindByName("cardnumber").Fill("424242424242424242");
await frame.FindByName("exp-date").Click();
await page.Keyboard("1030");
await frame.FindByName("cvc").Fill("123");
await page.FindById("stripe-checkout-form").FindButton().Click();

 
//10.  Order Status Page - Success
await page.WaitFor("http.*/web/payment-pending.*", CancelToken);
await page.WaitFor("http.*/web/payment-success.*", CancelToken);
await Sleep(1000);
await Screenshot(page);

//11.  Kyc approval
await Sleep(1000);
Print("Starting KYC...");
var orderRef = TestEnv.TestOrder.OrderRef;
var number = TestEnv.TestOrder.PhoneNumber;
await page.FindButton().FilterByText(page, "Confirm your identity").Click();

await Pause();

await DoKyc(KYC_URL, kycStatus, orderRef, number, cnic);
await Sleep(1000);

//12. Shipment
await Sleep(1000);
Print("Starting Delivery...");
var laas = LAAS_URL.Replace("{{orderRef}}", orderRef);
var shipmentRef = await GetShipmentRef(laas, orderRef);
TestEnv.TestOrder.ShipmentReference = shipmentRef;
Print($"Shipment reference : {shipmentRef}");

await Sleep(1000);
var riderUrl = RIDER_URL.Replace("{{shipmentReference}}", shipmentRef);
var refNum = await GetRefNum(riderUrl,  shipmentRef);
Print($"RefNum : {refNum}");
TestEnv.TestOrder.RefNum = refNum;

await Sleep(1000);
var iccid = await GetIccid(IMS_URL);
TestEnv.TestOrder.Iccid = iccid;
Print($"ICCID : {iccid}");

Print("Updating status to READY");
var status = "READY";
var statusId = 8;
await ShipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await Sleep(3000);

Print("Updating status to SHIPPED");
status = "SHIPPED";
statusId = 18;
await ShipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await Sleep(3000);

Print("Updating status to DELIVERED");
status = "DELIVERED";
statusId = 10;
await ShipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await Sleep(1500);

await Screenshot(page);

//13. Refresh the page to get the Sim activate button
Print("Refreshing page to check if the sim can be activated");
await Sleep(1500);
page.Refresh();

await page.FindButton().FilterByText(page, "Activate your SIM").Click();

await page.FindById("icc_id").FindTextBox().Fill(iccid);
await page.FindButton().FilterByText(page, "Activate Sim Card").Click();

//14. Refresh the page to get the Sim activate button
foreach (var i in Enumerable.Range(1,10)){
    Print("Sleeping for 5 sec before Refreshing page to check if the sim was activated");
    await Sleep(5000);
    page.Refresh();
    var t = page.FindByText("SIM Activated");
    if (t != null)
       break;
}

//-------------- Utility functions -----------------------

string getRandomMobile(string prefix){
     var sb = new StringBuilder(prefix);
     var r = new Random();
     var length = 11 - prefix.Length;
     for (int i = 0; i < length; i++) {
         sb.Append(r.Next(1, 10));
     }
        
    return sb.ToString();
}

string getRandomCnic(){
//1234512345671
  var sb = new StringBuilder();
  var r = new Random();
  for (int i = 0; i < 13; i++) {
      sb.Append(r.Next(1, 10));
  }

  return sb.ToString();

}
