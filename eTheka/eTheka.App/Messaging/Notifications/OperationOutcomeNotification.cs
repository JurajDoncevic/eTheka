using eTheka.Base;
using MediatR;

namespace eTheka.App.Messaging.Notifications;
public class OperationOutcomeNotification : INotification
{
    private readonly ResultTypes _resultType;
    private readonly string _message;

    public OperationOutcomeNotification(ResultTypes resultType, string message)
    {
        _resultType = resultType;
        _message = message;
    }

    public bool IsSuccess => _resultType == ResultTypes.SUCCESS;
    public bool IsFailure => _resultType == ResultTypes.FAILURE;
    public bool IsException => _resultType == ResultTypes.EXCEPTION;

    public ResultTypes ResultType => _resultType;

    public string Message => _message;
}
