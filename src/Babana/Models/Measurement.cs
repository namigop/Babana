using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaywrightTest.Models;

public class Measurement {
    public Measurement(string name) {
        Name = name;
    }

    public string Name { get; }
    public List<float> Raw { get; } = new();
    public float Average {
        get => Raw.Any() ? Raw.Average() : -1;
    }
    public float Min {
        get => Raw.Any() ? Raw.Min() : -1;
    }
    public float Max {
        get => Raw.Any() ? Raw.Max() : -1;
    }
    public float P90 {
        get => Raw.Any()  ? CalculatePercentile(Raw, 90) : -1;
    }

    public void Add(float i) {
        if (i > 0) {
            Raw.Add(i);
        }
    }

    private float CalculatePercentile(List<float> points, int percentile) {
        var pt = points.Count * (percentile / 100.0);
        var index = (int)Math.Round(pt, MidpointRounding.ToZero);
        return points[index];
    }

}
