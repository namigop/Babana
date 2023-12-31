
//********************************************
// This is a script executes the New Number flow
// automatically doing KYC, and sim activation
//********************************************

//modify these settings to match your screen resolution
var BROWSER_HEIGHT = 880;
var BROWSER_WIDTH = 1512;

var r = new Random();
var email = $"foo{r.Next(50, 1000)}@bar.com";
 
var plan = "30 GB";
var cnic = "1234512345671";
var contactNumber = "03123456789";
var nationality = "Pakistani";

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
var area = "17 Mile";
var landmark = "Red door with a golden door knob";
var deliveryInstruction = "Ring the doorbell and sing";
var deliveryDate = "16/06/2023";
var deliveryTime = "12:00 AM - 11:59 PM";
var kycStatus = "109";
// ---------------------------------

//--- Constants ------------------
var CHECKOUT_URL = "https://qpkvc-webfront.onic.com.pk/web/pre-checkout?reset=true"; 
var OTP_URL = "http://qpkvc-notify.onic.com.pk/api/v1/unified_ui/notification/admin/logs?idisplayStart=0&idisplayLength=10";
var KYC_URL = "http://qpkvc-kyc-bvs.onic.com.pk/mno/conn/v1/kyc/notif";
var LAAS_URL = "http://qpkvc-logistics-singpost.onic.com.pk/v1/internal/shipments?orderReference={{orderRef}}";
var RIDER_URL = "http://qpkvc-logistics-riders.onic.com.pk/v1/internal/shipments/{{shipmentReference}}/refnum";
var IMS_URL = "http://qpkvc-inventory.onic.com.pk/v2/internal/product-variant-identifiers?pviType=physicalSim&limit=10&page=1&status=available";
var RIDER_NOTIF_URL = "http://qpkvc-logistics-riders.onic.com.pk/v1/external/notification";

var NEXT ="Next";
var CONTINUE = "Continue";
var SLOMO_MSEC = 300;
var HEADLESS = false;
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
var TEST_ID_PROVINCE = "province";
var TEST_ID_CITY = "city"; 
var TEST_ID_AREA = "area"; 
var TEST_ID_ZIPCODE = "delivery_zip_code"; 
var TEST_ID_LANDMARK = "delivery_landmark"; 
var TEST_ID_DELIVERY_INSTRUCTION = "delivery_instructions"; 
var TEST_ID_DELIVERY_SLOTS = "delivery_date,delivery_slot_full_data";

//---- start of test execution ---------

print("Test is starting...");
TestEnv.TestOrder.Email = email;
var run = setup()
          .slomo(SLOMO_MSEC)
          .headless(HEADLESS)
          .browserHeight(BROWSER_HEIGHT)
          .browserWidth(BROWSER_WIDTH)
          .chromium();

var page = await run.begin(TestEnv);

//1. Checkout Page
await page.open(CHECKOUT_URL);
await page.findById(TEST_ID_PLANCARD)
          .filterByText(page, plan)
          .findButton()
          .click();

await sleep(1000);

//2. Addon Page
await page.waitFor("http.*/addon-selection");
await page.findButton()
          .filterByText(page, NEXT)
          .click();

//3. Signup page
await page.findTextBox()
          .fill(email);
await page.findButton()
          .filterByText(page, CONTINUE)
          .click();


//4. OTP page
await sleep(300);
var otp = await getOtp(OTP_URL, email);
page.findTextBox()
    .fillMany(otp);
await page.findButton()
          .filterByText(page, NEXT)
          .click();

 
//5. New Number page
await sleep(1000);
await page.findById("modal-button")
          .filterByText(page, OK)
          .click();
await page.findButton()
          .filterByText(page, NEXT)
          .click();


await pause();

//6. Personal Details page
await page.waitFor("http.*web/personal-details", CancelToken);
await page.findByPlaceholder(PLACEHOLDER_FIRSTNAME).fill(firstName);
await page.findByPlaceholder(PLACEHOLDER_MIDDLENAME).fill(middleName);
await page.findByPlaceholder(PLACEHOLDER_LASTNAME).fill(lastName);

await page.findById(TEST_ID_DOB_DAY).findByText(PLACEHOLDER_DAY).click();
await page.keyboard(dobDay);
await page.findById(TEST_ID_DOB_MONTH).findByText(PLACEHOLDER_MONTH).click();
await page.keyboard(dobMonth);
await page.findById(TEST_ID_DOB_YEAR).findByText(PLACEHOLDER_YEAR).click();
await page.keyboard(dobYear);
 
await page.findById(TEST_ID_CONTACT_NUMBER).findTextBox().fill(contactNumber);   
await page.findById(TEST_ID_NATIONALITY).click();
await page.keyboard(nationality);   
await page.findById(TEST_ID_CNIC).findTextBox().fill(cnic);
await page.findById(TEST_ID_CNIC_RECONFIRM).findTextBox().fill(cnic);

await page.findById(TEST_ID_CNIC_DOB_DAY).click();
await page.keyboard(cnic_dobDay);   
await page.findById(TEST_ID_CNIC_DOB_MONTH).click();
await page.keyboard(cnic_dobMonth);   
await page.findById(TEST_ID_CNIC_DOB_YEAR).click();
await page.keyboard(cnic_dobYear);   

await page.findById(TEST_ID_ADDRESS1).findTextBox().fill(address1);
await page.findById(TEST_ID_ADDRESS2).findTextBox().fill(address2);
await page.findById(TEST_ID_PROVINCE).click();
await page.keyboard(province);
await page.findById(TEST_ID_CITY).click();
await page.keyboard(city);
await page.findById(TEST_ID_AREA).click();
await page.keyboard(area);
await page.findById(TEST_ID_LANDMARK).findTextBox().fill(landmark);
await page.findById(TEST_ID_DELIVERY_INSTRUCTION).findTextBox().fill(deliveryInstruction);
await page.findById(TEST_ID_DELIVERY_SLOTS)
          .findButton()
          .filterByText(page, deliveryDate)
          .click();

await page.findById(TEST_ID_DELIVERY_SLOTS)
          .findButton()
          .filterByText(page, deliveryTime)
          .click();
await page.findButton().filterByText(page, NEXT).click();

//7. Order Summary Page
await page.waitFor("http.*web/order-summary", CancelToken);
await page.findButton().filterByText(page, PLACE_ORDER).click();

//8. Payfast Page
await page.waitFor("http.*Payfast/CardInfo", CancelToken);
await page.findListItem().filterByText(page, "Card Payment").Locator("nth=1").click();
await page.findButton().filterByText(page, "Make Payment").click();

//9. Emulator page
await page.waitFor("http.*mastercard/v2/prompt", CancelToken);
await page.findByText("Submit").click();
await sleep(1000);
await page.findByText("Back to Merchant").click();

//10.  Order Status Page - Success
await page.waitFor("http.*/web/payment-pending.*", CancelToken);
await page.waitFor("http.*/web/payment-success.*", CancelToken);
await sleep(1000);
await screenshot(page);

//11.  Kyc approval
await sleep(1000);
print("Starting KYC...");
var orderRef = TestEnv.TestOrder.OrderRef;
var number = TestEnv.TestOrder.PhoneNumber;
await doKyc(KYC_URL, kycStatus, orderRef, number, cnic);
await sleep(1000);

//12. Shipment
await sleep(1000);
print("Starting Delivery...");
var laas = LAAS_URL.Replace("{{orderRef}}", orderRef);
var shipmentRef = await getShipmentRef(laas, orderRef);
TestEnv.TestOrder.ShipmentReference = shipmentRef;
print($"Shipment reference : {shipmentRef}");

await sleep(1000);
var riderUrl = RIDER_URL.Replace("{{shipmentReference}}", shipmentRef);
var refNum = await getRefNum(riderUrl,  shipmentRef);
print($"RefNum : {refNum}");
TestEnv.TestOrder.RefNum = refNum;

await sleep(1000);
var iccid = await getIccid(IMS_URL);
TestEnv.TestOrder.Iccid = iccid;
print($"ICCID : {iccid}");

print("Updating status to READY");
var status = "READY";
var statusId = 8;
await shipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await sleep(3000);

print("Updating status to SHIPPED");
status = "SHIPPED";
statusId = 18;
await shipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await sleep(3000);

print("Updating status to DELIVERED");
status = "DELIVERED";
statusId = 10;
await shipmentUpdate(RIDER_NOTIF_URL, status, statusId, iccid, refNum);
await sleep(1500);

await screenshot(page);

//13. Refresh the page to get the Sim activate button
print("Refreshing page to check if the sim can be activated");
await sleep(1500);
page.refresh();

await page.findButton().filterByText(page, "Activate your SIM").click();

await page.findById("icc_id").findTextBox().fill(iccid);
await page.findButton().filterByText(page, "Activate Sim Card").click();

