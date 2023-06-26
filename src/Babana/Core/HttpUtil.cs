using System.Collections.Generic;
using System.Net.Http;

namespace PlaywrightTest.Core;

public static class HttpUtil {
    public static IEnumerable<string> Contentheaders = new List<string>() {
        "Allow",
        "Content-Disposition",
        "Content-Encoding",
        "Content-Language",
        "Content-Length",
        "Content-Location",
        "Content-MD5",
        "Content-Range",
        "Content-Type",
        "Expires",
        "Last-Modified"
    };


    public static HttpMethod GetMethod(string method) {
        if (method == "GET")
            return HttpMethod.Get;
        if (method == "PUT")
            return HttpMethod.Put;
        if (method == "POST")
            return HttpMethod.Post;
        if (method == "PATCH")
            return HttpMethod.Patch;
        if (method == "DELETE")
            return HttpMethod.Delete;
        if (method == "OPTIONS")
            return HttpMethod.Options;

        return HttpMethod.Trace;
    }
}