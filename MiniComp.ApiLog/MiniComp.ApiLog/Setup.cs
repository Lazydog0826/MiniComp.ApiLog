using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniComp.Core.App;

namespace MiniComp.ApiLog;

public static class Setup
{
    public static IServiceCollection AddApiLog(this IServiceCollection services)
    {
        services.AddScoped<IApiLogService, ApiLogService>();
        return services;
    }

    public static RecordLogDelegate ConsoleLogger = async model =>
    {
        var logger = WebApp.ServiceProvider.GetRequiredService<ILogger<ApiLogService>>();
        var log = new StringBuilder();
        foreach (var prop in model.GetType().GetProperties())
        {
            log.Append($"{prop.Name, -5}:\t{prop.GetValue(model)}\n");
        }
        logger.LogError("{StringBuilder}", log.ToString());
        await Task.CompletedTask;
    };
}
