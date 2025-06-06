# MiniComp.ApiLog
```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<RecordRequestFilter>();
});
builder
    .Services.AddApiLog()
    .Configure<RecordLogEvent>(opt =>
    {
        opt.Event += Setup.AnsiConsoleLogger;
    });
var app = builder.Build();
HostApp.RootServiceProvider = app.Services;
app.Use(
    async (context, next) =>
    {
        var apiLogService = context.RequestServices.GetRequiredService<IApiLogService>();
        try
        {
            await next.Invoke();
        }
        catch (CustomException ce)
        {
            var res = ce.GetWebApiResponse();
            apiLogService.SetExceptionResponseResult(ce, res, false, res.Code);
        }
        catch (Exception ex)
        {
            var res = WebApiResponse.Error("系统内部异常");
            apiLogService.SetExceptionResponseResult(ex, res, true, res.Code);
        }
        finally
        {
            await apiLogService.SaveApiLogAsync();
        }
    }
);
```
