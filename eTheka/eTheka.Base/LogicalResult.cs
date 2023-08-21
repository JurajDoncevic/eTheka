using System;
using System.Collections.Generic;

namespace eTheka.Base;

/// <summary>
/// Logical result of an operation
/// </summary>
public readonly struct Result : IResult
{
    private readonly string _message;
    private readonly ResultTypes _resultType;
    private readonly Exception? _exception;

    /// <summary>
    /// Is the operation a succcess?
    /// </summary>
    public bool IsSuccess => _resultType == ResultTypes.SUCCESS;
    /// <summary>
    /// Is the operation a failure?
    /// </summary>
    public bool IsFailure => _resultType == ResultTypes.FAILURE;
    /// <summary>
    /// Did the operation throw an exception?
    /// </summary>
    public bool IsException => _resultType == ResultTypes.EXCEPTION;
    /// <summary>
    /// Has the result an exception?
    /// </summary>
    public bool HasException => _exception != null;

    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType => _resultType;
    /// <summary>
    /// Possible exception - don't use unless result type is EXCEPTION
    /// </summary>
    public Exception Exception => _exception!;
    /// <summary>
    /// Result message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resultType">Result type</param>
    /// <param name="message">Result message</param>
    /// <param name="exception">Possible exception</param>
    internal Result(ResultTypes resultType, string? message = null, Exception? exception = null)
    {
        _exception = exception;
        // if no exception is given then, it's a failure - just in case
        _resultType = resultType == ResultTypes.EXCEPTION && _exception == null ? ResultTypes.FAILURE : resultType;

        // if no message is given, set the message according to the result type
        _message = message ?? resultType switch
        {
            ResultTypes.SUCCESS => "Operation successful",
            ResultTypes.FAILURE => "Operation failed",
            ResultTypes.EXCEPTION => $"Operation failed with exception: {exception!.Message}", // can't be null at this point; takes exception message
            _ => "UNKNOWN" // never gets to this, just to stop the warn
        };
    }

    /// <summary>
    /// Implicit bool operator
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator bool(Result result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a bool into a Result
    /// </summary>
    /// <param name="isSuccess"></param>
    public static implicit operator Result(bool isSuccess)
        => isSuccess
        ? Results.OnSuccess()
        : Results.OnFailure();

    public override bool Equals(object? obj)
    {
        return obj is Result result &&
               _message == result._message &&
               _resultType == result._resultType &&
               EqualityComparer<Exception?>.Default.Equals(_exception, result._exception);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_message, _resultType, _exception);
    }

    public static bool operator ==(Result left, Result right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result left, Result right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Result creation and extension methods
/// </summary>
public static partial class Results
{
    /// <summary>
    /// Create success result
    /// </summary>
    /// <param name="message">Custom success message</param>
    /// <returns>Successful Result with data</returns>
    public static Result OnSuccess(string? message = null)
        => new Result(ResultTypes.SUCCESS, message);

    /// <summary>
    /// Create failure result
    /// </summary>
    /// <param name="message">Custom failure message</param>
    /// <returns>Failure Result</returns>
    public static Result OnFailure(string? message = null)
        => new Result(ResultTypes.FAILURE, message); // default give null for ref types

    /// <summary>
    /// Create exception result
    /// </summary>
    /// <param name="exception">Thrown exception during operation</param>
    /// <param name="message">Custom message</param>
    /// <returns>Exception Result</returns>
    public static Result OnException(Exception exception, string? message = null)
        => new Result(ResultTypes.EXCEPTION, message, exception);
}
