using Microsoft.Extensions.DependencyInjection;
using MiniComp.Core.App;
using MiniComp.Core.Extension;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Json;

namespace MiniComp.ApiLog;

public static class Setup
{
    private static readonly Lock Lock = new Lock();

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

    public static RecordLogDelegate WriteLogFile = async model =>
    {
        await Task.CompletedTask;
        lock (Lock)
        {
            var rootPath = Path.Join(HostApp.AppRootPath, "logs");
            var fileName = DateTimeExtension.Now().ToString("yyyy-MM-dd") + ".log";
            if (Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            var filePath = Path.Join(rootPath, fileName);
            File.AppendAllTextAsync(
                filePath,
                $"{DateTimeExtension.Now():yyyy-MM-dd HH:mm:ss}===================="
                    + Environment.NewLine
                    + JsonConvert.SerializeObject(model)
                    + $"======================================="
                    + Environment.NewLine
            );
        }
    };
}
