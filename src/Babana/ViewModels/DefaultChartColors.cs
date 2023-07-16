using SkiaSharp;

namespace PlaywrightTest.ViewModels;

public static class DefaultChartColors {
    public static string[] ColorsRaw = {
        "#e60049",
        "#0bb4ff",
        "#50e991",
        "#e6d800",
        "#9b19f5",
        "#ffa300",
        "#dc0ab4",
        "#b3d4ff",
        "#00bfa0",
        //  SKColor.Parse("#732C4C")
        //[#e60049", "#0bb4ff", "#50e991", "#e6d800", "#9b19f5", "#ffa300", "#dc0ab4", "#b3d4ff", "#00bfa0"]
    };
    public static SKColor[] Colors = {
        SKColor.Parse(ColorsRaw[0]),
        SKColor.Parse(ColorsRaw[1]),
        SKColor.Parse(ColorsRaw[2]),
        SKColor.Parse(ColorsRaw[3]),
        SKColor.Parse(ColorsRaw[4]),
        SKColor.Parse(ColorsRaw[5]),
        SKColor.Parse(ColorsRaw[6]),
        SKColor.Parse(ColorsRaw[7]),
        SKColor.Parse(ColorsRaw[8]),
      //  SKColor.Parse("#732C4C")
        //[#e60049", "#0bb4ff", "#50e991", "#e6d800", "#9b19f5", "#ffa300", "#dc0ab4", "#b3d4ff", "#00bfa0"]
    };

}
