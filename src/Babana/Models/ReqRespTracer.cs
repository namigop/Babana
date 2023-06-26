using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PlaywrightTest.Models;

public class ReqRespTracer {
    private const int MAX_CACHE_SIZE = 1000;
    private const int BATCH_SIZE = 10;
    private const int MAX_DB_TRACE_COUNT = 5000;

    //private IDocReqRespTraceRepo _repo;

    private List<ReqRespTraceData> _cache = new(MAX_CACHE_SIZE);
    private TaskCompletionSource<int> _bufferingCs = new();
    private int _readPos = 0;
    private int _writePos = 0;
    private static object key = new();

    public event EventHandler<ReqRespTraceData> Traced;

    private ReqRespTracer() {
        //_repo = null;
        ClearCache();
    }

    public static Lazy<ReqRespTracer> Instance = new(() => new ReqRespTracer());

    private void ClearCache() {
        _cache.Clear();
        for (var i = 0; i < MAX_CACHE_SIZE; i++)
            _cache.Add(null);
    }

    public static void Trace(
        Uri sourceUri,
        string requestMethod,
        string requestBody,
        string respBody,
        Dictionary<string, string> requestHeaders,
        Dictionary<string, string> responseHeaders,
        int responseStatusCode,
        long swElapsedMilliseconds) {
        var dto = new ReqRespTraceData {
            Timestamp = DateTime.Now,
            ElapsedMsec = Convert.ToUInt32(swElapsedMilliseconds),
            RequestBody = requestBody,
            ResponseBody = respBody,
            LastUpdated = DateTime.Now,
            RequestUri = sourceUri.AbsoluteUri,
            StatusCode = responseStatusCode.ToString(),
            RequestMethod = requestMethod
        };

        var reqHeaders = new Dictionary<string, string>();
        foreach (var h in requestHeaders)
            reqHeaders.Add(h.Key, h.Value);

        var respHeaders = new Dictionary<string, string>();
        foreach (var h in responseHeaders)
            respHeaders.Add(h.Key, h.Value);

        dto.RequestHeaders = reqHeaders;
        dto.ResponseHeaders = respHeaders;

        Instance.Value.Trace(dto);
    }

    public void Setup() {
        //_repo ??= repo;

        //Start the background task to read then write to DB
        Task.Run(async () => {
            while (true) {
                var targetPos = await _bufferingCs.Task;
                _bufferingCs = new TaskCompletionSource<int>();
                Flush(targetPos);
            }
        });

        Task.Run(Cleanup);
    }

    public void Flush(int targetPos) {
        var delta = GetDelta(targetPos, _readPos);
        for (var i = 0; i <= delta; i++) {
            var dto = _cache[_readPos];
            _cache[_readPos] = null;
            Save(dto);
            _readPos = MoveForward(_readPos, MAX_CACHE_SIZE);
        }
    }

    public void FlushAll() {
        Flush(_writePos);
    }

    public List<ReqRespTraceData> GetAll() {
        return
            // _repo.All()
            //   .Concat(_cache)
            _cache
                .Where(c => c != null)
                .OrderBy(c => c.Timestamp)
                .ToList();
    }

    private void Save(ReqRespTraceData dto) {
        if (dto != null) {
            //_repo.Create(dto);
        }
    }

    private static int MoveForward(int x, int max) {
        if (x == max - 1)
            return 0;
        return x + 1;
    }

    private static int GetDelta(int writePos, int readPos) {
        if (writePos > readPos)
            return writePos - readPos;

        if (writePos == readPos)
            return 0;

        return MAX_CACHE_SIZE - 1 - readPos + writePos;
    }

    public void Trace(ReqRespTraceData dto) {
        lock (key) {
            _cache[_writePos] = dto;
            if (GetDelta(_writePos, _readPos) >= BATCH_SIZE) _bufferingCs.SetResult(_writePos);

            _writePos = MoveForward(_writePos, MAX_CACHE_SIZE);
        }

        Traced?.Invoke(this, dto);
    }

    public void ClearAll() {
        lock (key) {
            ClearCache();
            _writePos = 0;
            _readPos = 0;
            //_repo.DeleteAll();
        }
    }

    public void Cleanup() {
        // var count = _repo.Count();
        // if ( count > MAX_DB_TRACE_COUNT) {
        //     var toRemove = _repo.All().Take(count - MAX_DB_TRACE_COUNT);
        //     foreach (var i in toRemove)
        //         _repo.Delete(i.DocId);
        //
        //     _repo.Shrink();
        // }
    }

    public static void Trace(
        Uri sourceUri,
        string requestMethod,
        string requestBody,
        string respBody,
        HttpRequestHeaders requestHeaders,
        HttpResponseHeaders responseHeaders,
        HttpStatusCode responseStatusCode,
        long swElapsedMilliseconds) {
        Dictionary<string, string> h = new();
        foreach (var d in requestHeaders) h.Add(d.Key, string.Join(",", d.Value));

        Dictionary<string, string> r = new();
        foreach (var d in responseHeaders) r.Add(d.Key, string.Join(",", d.Value));

        Trace(sourceUri, requestMethod, requestBody, respBody, h, r, (int)responseStatusCode, swElapsedMilliseconds);
    }
}