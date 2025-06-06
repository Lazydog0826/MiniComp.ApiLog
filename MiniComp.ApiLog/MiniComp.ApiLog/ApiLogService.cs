using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniComp.Core.App;
using MiniComp.Core.Extension;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace MiniComp.ApiLog;

public class ApiLogService : IApiLogService
{
    private readonly ApiLogModel _apiLogModel = new();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private bool _logIsActive = true;
    private object? _responseContent;
    private HttpStatusCode _httpStatusCode;
    private readonly HttpContext? _httpContext = WebApp.HttpContext;
    private IDictionary<string, object?> _request = new Dictionary<string, object?>();
    private readonly ILogger<ApiLogService> _logger;
    private readonly RecordLogEvent _recordLogEvent;

    public ApiLogService(ILogger<ApiLogService> logger, IOptions<RecordLogEvent> options)
    {
        _logger = logger;
        _recordLogEvent = options.Value;
    }

    public ApiLogModel GetApiLogModel() => _apiLogModel;

    public async Task SaveApiLogAsync()
    {
        _stopwatch.Stop();
        var controllerActionDescriptor = _httpContext?.GetMetaData<ControllerActionDescriptor>();
        if (_logIsActive == false || _httpContext == null || controllerActionDescriptor == null)
            return;

        _apiLogModel.BeginTime = DateTimeExtension.Now();
        _apiLogModel.ServerIp = _httpContext.GetServerIp();
        _apiLogModel.ClientIp = _httpContext.GetClientIp();
        _apiLogModel.Path = _httpContext.Request.Path.ToString();
        _apiLogModel.CodePath = controllerActionDescriptor.DisplayName ?? string.Empty;
        _apiLogModel.Method = _httpContext.Request.Method;
        _apiLogModel.RequestHeaders = JsonConvert.SerializeObject(_httpContext.Request.Headers);
        _apiLogModel.EndTime = DateTimeExtension.Now();
        _apiLogModel.ElapsedTime = _stopwatch.ElapsedMilliseconds;
        _apiLogModel.Parameter = JsonConvert.SerializeObject(_request);
        _apiLogModel.ResponseHeaders = JsonConvert.SerializeObject(_httpContext.Response.Headers);

        try
        {
            _apiLogModel.ApiLogId = YitIdHelper.NextId();
            WebApp.AddValue(nameof(ApiLogModel.ApiLogId), _apiLogModel.ApiLogId.ToString());
            await _recordLogEvent.OnRecordLogEventAsync(_apiLogModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "\n日志记录异常{}:\n{}",
                DateTimeExtension.Now().ToString("yyyy-MM-dd HH:mm:ss"),
                JsonConvert.SerializeObject(ex)
            );
        }
        finally
        {
            if (_responseContent != null)
            {
                _httpContext.Response.StatusCode = (int)_httpStatusCode;
                await _httpContext.Response.WriteAsJsonAsync(_responseContent);
            }
        }
    }

    public void SetExceptionResponseResult(
        Exception ex,
        object resObj,
        bool logIsActive,
        HttpStatusCode httpStatusCode
    )
    {
        _logIsActive = logIsActive;
        _apiLogModel.Exception = ex;
        _responseContent = resObj;
        _httpStatusCode = httpStatusCode;
    }

    public void SetRequest(IDictionary<string, object?> obj)
    {
        _request = obj;
    }
}
