using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MiniComp.ApiLog;

public interface IApiLogService
{
    /// <summary>
    /// 保存
    /// </summary>
    public Task SaveApiLogAsync();

    /// <summary>
    /// 获取日志数据
    /// </summary>
    /// <returns></returns>
    public ApiLogModel GetApiLogModel();

    /// <summary>
    /// 设置异常响应结果
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="resObj"></param>
    /// <param name="logIsActive"></param>
    /// <param name="httpStatusCode"></param>
    public void SetExceptionResponseResult(
        Exception ex,
        object resObj,
        bool logIsActive,
        HttpStatusCode httpStatusCode
    );

    /// <summary>
    /// 暂存请求参数
    /// </summary>
    /// <param name="obj"></param>
    public void SetRequest(IDictionary<string, object?> obj);
}
