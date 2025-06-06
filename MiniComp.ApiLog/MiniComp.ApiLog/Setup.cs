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

    public static IServiceCollection AddConsoleOutputLogger(this IServiceCollection services)
    {
        services.AddLogging();
        services.Configure<RecordLogEvent>(rle =>
        {
            rle.Event += async model =>
            {
                var logger = WebApp.ServiceProvider.GetRequiredService<Logger<ApiLogService>>();
                var log = new StringBuilder();
                foreach (var prop in model.GetType().GetProperties())
                {
                    log.Append($"{prop.Name, -5}:\t{prop.GetValue(model)}\n");
                }
                logger.LogError("{StringBuilder}", log);
                await Task.CompletedTask;
            };
        });
        return services;
    }
}
