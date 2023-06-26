using PlaywrightTest.Models;

namespace PlaywrightTest;

public static class Startup {
    public static void Start() {
        ReqRespTracer.Instance.Value.Setup();

        //
        // var numberFoo = 8000;
        // List<string> nums = new();
        // for (int i = numberFoo; i< 8999; i++) {
        //     nums.Add($"0313466{i}");
        // }
        //
        // System.IO.File.WriteAllLines("/Users/erik.araojo/Downloads/number_upload_sample.csv", nums);
    }
}