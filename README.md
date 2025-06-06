# MiniComp.ApiLog
```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<RecordRequestFilter>();
});
builder.Services.AddApiLog().AddConsoleOutputLogger();
builder.Services.Configure<RecordLogEvent>(opt =>
{
    opt.Event += async model =>
    {
        Console.WriteLine(model);
        await Task.CompletedTask;
    };
});
```
