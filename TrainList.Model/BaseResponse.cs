using System.Diagnostics;

namespace TrainList.Model;

public class BaseResponse<T> : IBaseResponse<T> 
{
    /// <summary>
    /// описание ошибки если в запросе что-то пошло не так
    /// </summary>
    public string Description { get; set; }
    public ActivityStatusCode StatusCode { get; set; }
    public T Data { get; set; }
}

public interface IBaseResponse<T> 
{
    T Data { get; }
    string Description { get; }
    ActivityStatusCode StatusCode { get; }
}