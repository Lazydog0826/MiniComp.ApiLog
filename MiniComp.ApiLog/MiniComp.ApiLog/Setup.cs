using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Json;

namespace MiniComp.ApiLog;

public static class Setup
{
    public static IServiceCollection AddApiLog(this IServiceCollection services)
    {
        services.AddScoped<IApiLogService, ApiLogService>();
        return services;
    }

    public static RecordLogDelegate AnsiConsoleLogger = async model =>
    {
        var json = new JsonText(JsonConvert.SerializeObject(model));
        AnsiConsole.Write(
            new Panel(json).Header("异常日志").Collapse().RoundedBorder().BorderColor(Color.Red)
        );
        await Task.CompletedTask;
    };
}
