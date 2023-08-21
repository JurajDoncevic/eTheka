using System;
using System.Threading.Tasks;

namespace eTheka.Base;
public partial class Results
{
    #region MATCH
    /// <summary>
    /// Match function to resolve the Result to a single type of data by result type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="onSuccess">Function to execute on success</param>
    /// <param name="onFailure">Function to execute on failure. First parameter is the message</param>
    /// <param name="onException">Function to execute on exception. First parameter is the message</param>
    /// <returns></returns>
    public static R Match<T, R>(this Result<T> result, Func<T, R> onSuccess, Func<string, R> onFailure, Func<Exception, string, R>? onException = null)
        => result switch
        {
            Result<T> { ResultType: ResultTypes.SUCCESS } r => onSuccess(r.Data),
            Result<T> { ResultType: ResultTypes.FAILURE } r => onFailure(r.Message),
            Result<T> { ResultType: ResultTypes.EXCEPTION } r => onException is not null ? onException.Invoke(r.Exception, r.Message) : onFailure.Invoke(r.Message),
            _ => throw new NotImplementedException()
        };

    /// <summary>
    /// Match function to resolve the Result to a single type of data by result type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="onSuccess">Function to execute on success</param>
    /// <param name="onFailure">Function to execute on failure. First parameter is the message</param>
    /// <param name="onException">Function to execute on exception. First parameter is the message</param>
    /// <returns></returns>
    public static R Match<R>(this Result result, Func<string, R> onSuccess, Func<string, R> onFailure, Func<Exception, string, R>? onException = null)
        => result switch
        {
            Result { ResultType: ResultTypes.SUCCESS } r => onSuccess(r.Message),
            Result { ResultType: ResultTypes.FAILURE } r => onFailure(r.Message),
            Result { ResultType: ResultTypes.EXCEPTION } r => onException is not null ? onException.Invoke(r.Exception, r.Message) : onFailure.Invoke(r.Message),
            _ => throw new NotImplementedException()
        };

    /// <summary>
    /// Async Match function to resolve the Result to a single type of data by result type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="onSuccess">Function to execute on success</param>
    /// <param name="onFailure">Function to execute on failure. First parameter is the message</param>
    /// <param name="onException">Function to execute on exception. First parameter is the message</param>
    /// <returns></returns>
    public async static Task<R> Match<T, R>(this Task<Result<T>> result, Func<T, R> onSuccess, Func<string, R> onFailure, Func<Exception, string, R>? onException = null)
        => await result switch
        {
            Result<T> { ResultType: ResultTypes.SUCCESS } r => onSuccess(r.Data),
            Result<T> { ResultType: ResultTypes.FAILURE } r => onFailure(r.Message),
            Result<T> { ResultType: ResultTypes.EXCEPTION } r => onException is not null ? onException.Invoke(r.Exception, r.Message) : onFailure.Invoke(r.Message),
            _ => throw new NotImplementedException()
        };

    /// <summary>
    /// Async Match function to resolve the Result to a single type of data by result type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="onSuccess">Function to execute on success</param>
    /// <param name="onFailure">Function to execute on failure. First parameter is the message</param>
    /// <param name="onException">Function to execute on exception. First parameter is the message</param>
    /// <returns></returns>
    public async static Task<R> Match<R>(this Task<Result> result, Func<string, R> onSuccess, Func<string, R> onFailure, Func<Exception, string, R>? onException = null)
        => await result switch
        {
            Result { ResultType: ResultTypes.SUCCESS } r => onSuccess(r.Message),
            Result { ResultType: ResultTypes.FAILURE } r => onFailure(r.Message),
            Result { ResultType: ResultTypes.EXCEPTION } r => onException is not null ? onException.Invoke(r.Exception, r.Message) : onFailure.Invoke(r.Message),
            _ => throw new NotImplementedException()
        };
    #endregion

    #region MAP
    /// <summary>
    /// Map on Result: M[T] -> (T -> R) -> M[R]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func">Mapping function</param>
    /// <returns></returns>
    public static Result<R> Map<T, R>(this Result<T> result, Func<T, R> func)
        => result.IsSuccess
            ? func(result.Data)
            : new Result<R>(default, result.ResultType, result.Message, result.Exception);

    /// <summary>
    /// Async Map on Result: M[T] -> (T -> R) -> M[R]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func">Mapping function</param>
    /// <returns></returns>
    public static async Task<Result<R>> Map<T, R>(this Task<Result<T>> result, Func<T, R> func) =>
        await result.MapTask(r => r.IsSuccess
            ? new Result<R>(func(r.Data), r.ResultType, r.Message, r.Exception)
            : new Result<R>(default, r.ResultType, r.Message, r.Exception));
    #endregion

    #region BIND

    /// <summary>
    /// Bind on Result: M -> (() -> M') -> M'
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func">Binding function</param>
    /// <returns></returns>
    public static Result Bind(this Result result, Func<Result> func)
        => result.IsSuccess
            ? func()
            : result;

    /// <summary>
    /// Async Bind on Result: M -> (() -> M') -> M'
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func">Bind
    public static async Task<Result> Bind(this Task<Result> result, Func<Task<Result>> func)
    {
        var awaitedResult = await result;
        return awaitedResult
            ? await func()
            : awaitedResult;
    }
    /// <summary>
    /// Bind on Result: M[T] -> (T -> M[R]) -> M[R]
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func">Bind
    public static Result<R> Bind<T, R>(this Result<T> result, Func<T, Result<R>> func)
        => result.IsSuccess
            ? func(result.Data)
            : new Result<R>(default, result.ResultType, result.Message, result.Exception);

    /// <summary>
    /// Async Bind on Result: M[T] -> (T -> M[R]) -> M[R]
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func">Bind
    public static async Task<Result<R>> Bind<T, R>(this Task<Result<T>> result, Func<T, Task<Result<R>>> func)
    {
        var awaitedResult = await result;
        return awaitedResult
            ? await func(awaitedResult.Data)
            : new Result<R>(default, awaitedResult.ResultType, awaitedResult.Message, awaitedResult.Exception);
    }

    /// <summary>
    /// Bind between Result and Result with data: M -> (() -> M[R]) -> M[R]
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Result<R> Bind<R>(this Result result, Func<Result<R>> func)
        => result.IsSuccess
            ? func()
            : new Result<R>(default, (ResultTypes)result.ResultType, (string)result.Message, (Exception)result.Exception);

    /// <summary>
    /// Async Bind between Result and Result with data: M -> (M -> M[R]) -> M[R]
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result<R>> Bind<R>(this Task<Result> result, Func<Task<Result<R>>> func)
    {
        var awaitedResult = await result;
        return awaitedResult.IsSuccess
            ? await func()
            : new Result<R>(default, (ResultTypes)awaitedResult.ResultType, (string)awaitedResult.Message, (Exception)awaitedResult.Exception);
    }

    /// <summary>
    /// Bind between Result and Result with data: M[T] -> (T -> M) -> M
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Result Bind<T>(this Result<T> result, Func<T, Result> func)
        => result.IsSuccess
            ? func(result.Data)
            : new Result(result.ResultType, result.Message, result.Exception);

    /// <summary>
    /// Async Bind between Result[T] and Result with data: M[T] -> (T -> M) -> M
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result> Bind<T>(this Task<Result<T>> result, Func<T, Task<Result>> func)
    {
        var awaitedResult = await result;
        return awaitedResult.IsSuccess
            ? await func(awaitedResult.Data)
            : new Result(awaitedResult.ResultType, awaitedResult.Message, awaitedResult.Exception);
    }

    /// <summary>
    /// Bind that accepts an operation returning a task-lifted Result[T]: M[T] -> (T -> Task[M[R]]) -> Task[M[R]]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result<R>> Bind<T, R>(this Result<T> result, Func<T, Task<Result<R>>> func)
    {
        return await result.Match(
            t => func(t),
            msg => Task.FromResult(new Result<R>(default, ResultTypes.FAILURE, msg)),
            (ex, msg) => Task.FromResult(new Result<R>(default, ResultTypes.EXCEPTION, msg, ex))
            );
    }

    /// <summary>
    /// Bind that accepts an operation returning a task-lowered Result[T]: Task[M[T]] -> (T -> M[R]) -> M[R]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Result<R> Bind<T, R>(this Task<Result<T>> result, Func<T, Result<R>> func)
    {
        var awaitedResult = result.Result;
        return awaitedResult.Match(
            t => func(t),
            msg => new Result<R>(default, ResultTypes.FAILURE, msg),
            (ex, msg) => new Result<R>(default, ResultTypes.EXCEPTION, msg, ex)
            );
    }

    /// <summary>
    /// Bind that accepts an operation returning a task-lifted Result: M -> (()-> Task[M]) -> Task[M]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result> Bind(this Result result, Func<Task<Result>> func)
    {
        return await result.Match(
            msg => func(),
            msg => Task.FromResult(new Result(ResultTypes.FAILURE, msg)),
            (ex, msg) => Task.FromResult(new Result(ResultTypes.EXCEPTION, msg, ex))
            );
    }

    /// <summary>
    /// Bind that accepts an operation returning a task-lowered Result: Task[M] -> (() -> M) -> M
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Result Bind(this Task<Result> result, Func<Result> func)
    {
        var awaitedResult = result.Result;
        return awaitedResult.Match(
            msg => func(),
            msg => new Result(ResultTypes.FAILURE, msg),
            (ex, msg) => new Result(ResultTypes.EXCEPTION, msg, ex)
            );
    }
    #endregion

    public static Result<TData> AsResult<TData>(Func<Result<TData>> operation)
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            return Results.OnException<TData>(ex);
        }
    }

    public static Result AsResult(Func<Result> operation)
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            return Results.OnException(ex);
        }
    }

    public static async Task<Result<TData>> AsResult<TData>(Func<Task<Result<TData>>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            return Results.OnException<TData>(ex);
        }
    }

    public static async Task<Result> AsResult(Func<Task<Result>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            return Results.OnException(ex);
        }
    }
}
