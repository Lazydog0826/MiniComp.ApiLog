using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using MiniComp.Core.App;
using MiniComp.Core.Extension;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Json;
using Yitter.IdGenerator;

namespace MiniComp.ApiLog;

public class ApiLogService(IOptions<RecordLogEvent> options) : IApiLogService
{
    private readonly ApiLogModel _apiLogModel = new();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private bool _logIsActive;
    private object? _responseContent;
    private HttpStatusCode _httpStatusCode;
    private readonly HttpContext? _httpContext = WebApp.HttpContext;
    private IDictionary<string, object?> _request = new Dictionary<string, object?>();
    private readonly RecordLogEvent _recordLogEvent = options.Value;

    public ApiLogModel GetApiLogModel() => _apiLogModel;

    public async Task SaveApiLogAsync()
    {
        _stopwatch.Stop();
        if (_logIsActive == false)
        {
            return;
        }

        var controllerActionDescriptor = _httpContext?.GetMetaData<ControllerActionDescriptor>();
        _apiLogModel.BeginTime = DateTimeExtension.Now();
        _apiLogModel.ServerIp = _httpContext?.GetServerIp() ?? string.Empty;
        _apiLogModel.ClientIp = _httpContext?.GetClientIp() ?? string.Empty;
        _apiLogModel.Path = _httpContext?.Request.Path.ToString() ?? string.Empty;
        _apiLogModel.CodePath = controllerActionDescriptor?.DisplayName ?? string.Empty;
        _apiLogModel.Method = _httpContext?.Request.Method ?? string.Empty;
        _apiLogModel.RequestHeaders = JsonConvert.SerializeObject(_httpContext?.Request.Headers);
        _apiLogModel.EndTime = DateTimeExtension.Now();
        _apiLogModel.ElapsedTime = _stopwatch.ElapsedMilliseconds;
        _apiLogModel.Parameter = JsonConvert.SerializeObject(_request);
        _apiLogModel.ResponseHeaders = JsonConvert.SerializeObject(_httpContext?.Response.Headers);

        try
        {
            WebApp.AddValue(nameof(ApiLogModel.ApiLogId), _apiLogModel.ApiLogId.ToString());
            await _recordLogEvent.OnRecordLogEventAsync(_apiLogModel);
        }
        catch (Exception ex)
        {
            var json = new JsonText(JsonConvert.SerializeObject(ex));
            AnsiConsole.Write(
                new Panel(json)
                    .Header($"异常日志 - {DateTimeExtension.Now():yyyy-MM-dd HH:mm:ss}")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Red)
            );
        }
    }

    public async Task SetExceptionAsync(
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
        if (logIsActive)
        {
            _apiLogModel.ApiLogId = YitIdHelper.NextId();
        }
        if (_httpContext != null)
        {
            await _httpContext.Response.WriteAsJsonAsync(resObj);
            _httpContext.Response.StatusCode = (int)_httpStatusCode;
        }
    }

    public void SetRequest(IDictionary<string, object?> obj)
    {
        _request = obj;
    }
}
