namespace MiniComp.ApiLog;

public delegate Task RecordLogDelegate(ApiLogModel apiLogModel);

public class RecordLogEvent
{
    public event RecordLogDelegate? Event;

    public Task OnRecordLogEventAsync(ApiLogModel apiLogModel)
    {
        return Event?.Invoke(apiLogModel) ?? Task.CompletedTask;
    }
}
