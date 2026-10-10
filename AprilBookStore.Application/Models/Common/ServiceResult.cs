namespace AprilBookStore.Application.Models.Common;

public class ServiceResult
{
    public bool Succeeded { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    public static ServiceResult Success() => new() { Succeeded = true };
    public static ServiceResult Failed(params string[] errors) => new() { Succeeded = false, Errors = errors.ToList() };
    public static ServiceResult Failed(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToList() };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static ServiceResult<T> Failed(params string[] errors) => new() { Succeeded = false, Errors = errors.ToList() };
    public new static ServiceResult<T> Failed(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToList() };
}
