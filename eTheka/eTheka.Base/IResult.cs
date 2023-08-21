using System;

namespace eTheka.Base;

/// <summary>
/// Result interface
/// </summary>
public interface IResult
{
    /// <summary>
    /// Is the operation a succcess?
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// Is the operation a failure?
    /// </summary>
    public bool IsFailure { get; }
    /// <summary>
    /// Did the operation throw an exception?
    /// </summary>
    public bool IsException { get; }
    /// <summary>
    /// Has the result an exception?
    /// </summary>
    public bool HasException { get; }
    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType { get; }
    /// <summary>
    /// Possible exception - don't use unless result type is EXCEPTION
    /// </summary>
    public Exception Exception { get; }
    /// <summary>
    /// Result message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Result interface for results containing data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
#pragma warning disable IDE1006
    public interface WithData<TData> : IResult
#pragma warning restore IDE1006
    {
        /// <summary>
        /// Has the result any data?
        /// </summary>
        public bool HasData { get; }
        /// <summary>
        /// Result data - don't use unless result type is SUCCESS
        /// </summary>
        public TData Data { get; }
    }

}
