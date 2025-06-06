namespace MiniComp.ApiLog;

public class ApiLogModel
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public long ApiLogId { get; set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime BeginTime { get; set; }

    /// <summary>
    /// 响应时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 运行时间（毫秒）
    /// </summary>
    public long ElapsedTime { get; set; }

    /// <summary>
    /// 接口地址
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 代码类路径
    /// </summary>
    public string CodePath { get; set; } = string.Empty;

    /// <summary>
    /// 接口方法
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 参数
    /// </summary>
    public string Parameter { get; set; } = string.Empty;

    /// <summary>
    /// 异常
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// 服务端IP
    /// </summary>
    public string ServerIp { get; set; } = string.Empty;

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string ClientIp { get; set; } = string.Empty;

    /// <summary>
    /// 请求头
    /// </summary>
    public string RequestHeaders { get; set; } = string.Empty;

    /// <summary>
    /// 响应头
    /// </summary>
    public string ResponseHeaders { get; set; } = string.Empty;
}
