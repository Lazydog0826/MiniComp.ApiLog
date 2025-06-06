using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MiniComp.Core.App;

namespace MiniComp.ApiLog;

public class RecordRequestFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var apiLogService = WebApp.ServiceProvider.GetService<IApiLogService>();
        apiLogService?.SetRequest(context.ActionArguments);
        await base.OnActionExecutionAsync(context, next);
    }
}
